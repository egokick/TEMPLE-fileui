using System.IO;
using fileui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace TEMPLE.Services
{
    public class FileDataService
    {
        public DatabaseService _databaseService;        
        public FileDataService(string dbConnection, string password)
        {
            _databaseService = new DatabaseService(dbConnection, password);
        }

        public async Task<bool> DatabaseInitialise()
        {
            try
            {
                var dbResult = await _databaseService.ExecuteQuery("show databases like 'temple';");
                // no schema so must create it
                if (dbResult.Rows.Count == 0)
                {
                    var schemaResult = await _databaseService.ExecuteQuery("CREATE SCHEMA `temple` DEFAULT CHARACTER SET utf8 ;");
                    var tableResult = await _databaseService.ExecuteQuery(@"CREATE TABLE `temple`.`file` (
                  `FileId` BIGINT NOT NULL AUTO_INCREMENT,                 
                  `Tag1` VARCHAR(45) NULL,
                  `Tag2` VARCHAR(45) NULL,
                  `Tag3` VARCHAR(45) NULL,
                  `Tag4` VARCHAR(45) NULL,
                  `Tag5` VARCHAR(45) NULL,
                  `Tag6` VARCHAR(45) NULL,
                  `Tag7` VARCHAR(45) NULL,
                  `Tag8` VARCHAR(45) NULL,
                  `Tag9` VARCHAR(45) NULL,
                  `Tag10` VARCHAR(45) NULL,
                  `Path` VARCHAR(500) NULL,
                  `SHA256` VARCHAR(64) NULL,                  
                  PRIMARY KEY (`FileId`),
                  UNIQUE INDEX `FileId_UNIQUE` (`FileId` ASC),
                  INDEX `Path` (`Path` ASC),
                  INDEX `Tag1` (`Tag1` ASC),
                  INDEX `Tag2` (`Tag2` ASC),
                  INDEX `Tag3` (`Tag3` ASC));");
                }

                var checkTableResult = await _databaseService.ExecuteQuery("SELECT COUNT(*) FROM temple.file;");
                if (checkTableResult != null && checkTableResult.Rows.Count > 0)
                {
                    Console.WriteLine($"temple.file row count: {checkTableResult.Rows[0].ItemArray[0]}");
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // todo sqlquery batcher optimize for 150? or db speed i dunno
        public async Task<bool> SyncFolderPaths(string[] filePaths)
        {
            try
            {
                var sqlQuery = new StringBuilder();
                sqlQuery.AppendLine("INSERT INTO temple.file(Path)");
                foreach (var path in filePaths)
                {                    
                    var safePath = path.Replace("\\", "\\\\");
                    safePath = safePath.Replace("'", @"\'");
                    if (sqlQuery.Length > 35) sqlQuery.AppendLine(" union all");
                    sqlQuery.Append($"SELECT '{safePath}' FROM (SELECT COUNT(*) as Count FROM temple.file WHERE Path = '{safePath}') as t WHERE t.Count = 0");

                    if(sqlQuery.Length > 10000)
                    {
                        
                        var innerResult = await _databaseService.ExecuteQuery($"{sqlQuery}");
                        
                        sqlQuery.Clear();
                        sqlQuery.AppendLine("INSERT INTO temple.file(Path)");
                    }
                }
                sqlQuery.Append(";");

                var insertResult = await _databaseService.ExecuteQuery($"{sqlQuery}");

                if (insertResult is null || insertResult.HasErrors) return false;

                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"ERROR {ex.Message}");
                return false;
            }
        }

        // expects you to have ran SyncFolderPaths already
        // can take a long time to complete if you have a lot of files!
        public async Task<bool> SyncFileHash(string folder = null)
        {
            // all files
            if (string.IsNullOrEmpty(folder))
            {
                var sqlQuery = "SELECT Path FROM temple.file;";
                var pathsResult = await _databaseService.ExecuteQuery<string>(sqlQuery);

                var sqlDelete = new StringBuilder();
                var sqlUpdate = new StringBuilder();

                foreach(var path in pathsResult)
                {
                    var safePath = path.Replace("\\", "\\\\");
                    safePath = safePath.Replace("'", @"\'");
                    // check if file exists
                    if (!File.Exists(path))
                    {
                        sqlDelete.AppendLine($"DELETE FROM temple.file WHERE Path = '{safePath}';");
                        continue;
                    }

                    // get file size 
                    var f = new FileInfo(path);
                    // 1MB
                    if (f.Length < 1000000)
                    {
                        using var SHA256 = SHA256Managed.Create();
                        using FileStream fileStream = File.OpenRead(path);
                        var fileSHA256 = Convert.ToBase64String(SHA256.ComputeHash(fileStream));
                        sqlUpdate.AppendLine($"UPDATE temple.file SET sha256 = '{fileSHA256}' WHERE Path = '{safePath}';");
                    }
                    else
                    {
                        sqlUpdate.AppendLine($"UPDATE temple.file SET sha256 = 'greater than 1000000 bytes' WHERE Path = '{safePath}';");
                    }
                }
                
                if(sqlDelete.Length > 0) _ = await _databaseService.ExecuteQuery($"{sqlDelete}");
                if (sqlUpdate.Length > 0) _ = await _databaseService.ExecuteQuery($"{sqlUpdate}");

            }
            else // just files in this directory
            {
                Console.WriteLine("todo");
                throw new Exception("todo");
            }

            return true;
        }

        public async Task<bool> TagFilesInFolder(string folderPath, List<FileTag> fileTags)
        {            
            // yeah 8 slashes... obviously
            var folderPathSuperSafe = folderPath.Replace("\\", "\\\\\\\\");
            folderPathSuperSafe = folderPathSuperSafe.Replace("'", @"\'");
            var sqlAddTag = new StringBuilder();
            sqlAddTag.AppendLine("UPDATE temple.file");
            // if no tags are enabled then clear all tags for all files in the folder
            if (fileTags.All(x => !x.Enabled)) 
            { 
                sqlAddTag.AppendLine("SET Tag1 = null, Tag2 = null, Tag3 = null, Tag4 = null, Tag5 = null, Tag6 = null, Tag7 = null, Tag8 = null, Tag9 = null, Tag10 = null");
            }
            else
            {
                sqlAddTag.Append("SET ");
                int i = 1;
                foreach (var tag in fileTags.Where(x=>x.Enabled))
                {                    
                    if(i>1) sqlAddTag.Append(", ");
                    sqlAddTag.Append($"Tag{tag.Order} = '{tag.TagName}'");
                    i++;
                }
                sqlAddTag.AppendLine("");
            }            

            sqlAddTag.AppendLine($"WHERE Path like '{folderPathSuperSafe}%';");            
            _ = await _databaseService.ExecuteQuery($"{sqlAddTag}");

            return true;
        }
    }
}
