﻿// Copyright (c) Arctium Emulation.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Logging.IO
{
    public class FileWriter : IDisposable
    {
        FileStream logStream;

        public FileWriter(string directory, string file)
        {
            logStream = new FileStream($"{directory}/{DateTime.Now:yyyy-MM-dd_hh-mm-ss}_{file}", FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096, true);
        }

        public async Task Write(string logMessage)
        {
            var logBytes = Encoding.Unicode.GetBytes(logMessage + "\r\n");

            await logStream.WriteAsync(logBytes, 0, logBytes.Length);
            await logStream.FlushAsync();
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    logStream.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
