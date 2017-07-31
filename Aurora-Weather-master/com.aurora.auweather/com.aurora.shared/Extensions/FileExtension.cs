﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Com.Aurora.Shared.Extensions
{
    public static class FileExtension
    {
        public static async Task<List<StorageFile>> GetAllFilesAsync(this StorageFolder folder)
        {
            var list = new List<StorageFile>();
            list.AddRange(await folder.GetFilesAsync());
            var folders = await folder.GetFoldersAsync();
            if (!folders.IsNullorEmpty())
            {
                foreach (var item in folders)
                {
                    list.AddRange(await item.GetAllFilesAsync());
                }
            }
            return list;
        }
    }
}
