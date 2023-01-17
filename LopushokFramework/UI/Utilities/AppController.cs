using LopushokFramework.LopushokDataBase;
using System;
using System.IO;
using System.Windows.Controls;

namespace LopushokFramework.UI.Utilities
{
    static class AppController
    {
        public static Frame Frame { get; set; }

        private static LopushokContext _context;
        public static LopushokContext DbContext => _context ?? (_context = new LopushokContext());

        public static string PathToPictures => Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../UI/Pictures"));
    }
}