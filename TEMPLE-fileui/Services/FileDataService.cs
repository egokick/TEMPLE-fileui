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
                  `Path` VARCHAR(200) NULL,
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
            var sqlQuery = new StringBuilder();
            sqlQuery.AppendLine("INSERT INTO temple.file(Path)");            
            foreach(var path in filePaths)
            {                
                if (sqlQuery.Length > 35) sqlQuery.AppendLine(" union all");
                sqlQuery.Append($"SELECT '{path}' FROM (SELECT COUNT(*) as Count FROM temple.file WHERE Path = '{path}') as t WHERE t.Count = 0");
            }
            sqlQuery.Append(";");

            sqlQuery = sqlQuery.Replace("\\", "\\\\");
            var insertResult = await _databaseService.ExecuteQuery($"{sqlQuery}");

            if (insertResult is null || insertResult.HasErrors) return false;

            return true;
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
                    // check if file exists
                    if (!File.Exists(path))
                    {
                        sqlDelete.Append($"DELETE FROM temple.file WHERE Path = '{safePath}';");
                        continue;
                    }

                    using var SHA256 = SHA256Managed.Create();
                    using FileStream fileStream = File.OpenRead(path);
                    var fileSHA256 = Convert.ToBase64String(SHA256.ComputeHash(fileStream));
                    sqlUpdate.Append($"UPDATE temple.file SET sha256 = {fileSHA256} WHERE Path = {safePath};");
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
    }
}
