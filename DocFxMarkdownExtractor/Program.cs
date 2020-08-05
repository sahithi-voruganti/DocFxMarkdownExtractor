using DocFxMarkdownExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;

namespace DocFxTocGenerate
{
    class Program
    {
        public static string rootFolder;

        static void Main(string[] args)
        {
            Console.WriteLine("Enter Source folder path");
            string src = Console.ReadLine();
            Console.WriteLine("Enter Destination folder path");
            string dest = Console.ReadLine();
            
            var myFileUtil = new FileUtility(src, dest);
            myFileUtil.WalkPath();

            rootFolder = dest;
            var tocRootItems = new TocItem();
            System.IO.DirectoryInfo rootDir = new System.IO.DirectoryInfo(rootFolder);
            WalkDirectoryTree(rootDir, ref tocRootItems);

            var serializer = new Serializer();
            string output = null;
            if(string.IsNullOrEmpty(tocRootItems.name))
            {
                output = serializer.Serialize(tocRootItems.items);
            } else
            {
                output = serializer.Serialize(tocRootItems.items);
            }

            System.IO.File.WriteAllText(dest + "/toc.yml", output);
        }


        static void WalkDirectoryTree(System.IO.DirectoryInfo folder, ref TocItem yamlNodes)
        {
            System.IO.DirectoryInfo[] subDirs = null;
            
            var files = GetMarkdownFiles(folder);
            foreach(var file in files)
            {
                var newTocItem = new TocItem();
                newTocItem.name = Path.GetFileNameWithoutExtension(file.FullName);
                newTocItem.href = GetRelativePath(file.FullName, rootFolder);
                yamlNodes.AddItem(newTocItem);
            }
            
            // Now find all the subdirectories under this directory.
            subDirs = folder.GetDirectories();

            foreach (System.IO.DirectoryInfo dirInfo in subDirs)
            {
                if (dirInfo.Name.StartsWith(".")) { continue; }

                var subFiles = GetFiles(dirInfo);
                if (!subFiles.Any() && !dirInfo.GetDirectories().Any()) { continue; }

                var newTocItem = new TocItem();
                newTocItem.name = UppercaseFirst(dirInfo.Name.Replace("-", " "));

                if (subFiles.Count >= 1 && dirInfo.GetDirectories().Length == 0)
                {
                    newTocItem.href = GetRelativePath(subFiles[0].FullName, rootFolder);
                }
                else if (subFiles.Count >= 1 && dirInfo.GetDirectories().Length > 0)
                {
                    newTocItem.href = GetRelativePath(subFiles[0].FullName, rootFolder);
                    WalkDirectoryTree(dirInfo, ref newTocItem);
                }
                else
                {
                    newTocItem.topichref = GetRelativePath(dirInfo.FullName, rootFolder) + @"/";
                    WalkDirectoryTree(dirInfo, ref newTocItem);
                }
                yamlNodes.AddItem(newTocItem);
            }
        }

        private static List<FileInfo> GetFiles(DirectoryInfo folder)
        {
            var files = folder.GetFiles("*.md").OrderBy(f => f.Name)
                .Where(f => f.Name.ToLower() != "index.md" && f.Name.ToLower() == "readme.md")
                .ToList();
            if (files == null)
            {
                return null;
            }

            return files;
        }

        private static List<FileInfo> GetMarkdownFiles(DirectoryInfo folder)
        {
            var files = folder.GetFiles("*.md").OrderBy(f => f.Name)
                .Where(f => f.Name.ToLower() != "index.md" && f.Name.ToLower() != "readme.md")
                .ToList();
            if (files == null)
            {
                return null;
            }

            return files;
        }

        private static string GetCleanedFileName(FileInfo fi)
        {
            var cleanedName = fi.Name;

            if (fi.Name.ToLower() == "index.md")
            {
                cleanedName = fi.DirectoryName
                    .Substring(fi.DirectoryName.LastIndexOf("\\") + 1);
            }
            return UppercaseFirst(cleanedName.Replace("-", " ").Replace(".md", ""));
        }

        static string GetRelativePath(string filePath, string sourcePath = null)
        {
            string currentDir = sourcePath ?? Environment.CurrentDirectory;
            DirectoryInfo directory = new DirectoryInfo(currentDir);
            FileInfo file = new FileInfo(filePath);

            string fullDirectory = directory.FullName;
            string fullFile = file.FullName;

            if (!fullFile.StartsWith(fullDirectory))
            {
                throw new InvalidOperationException("Unable to make relative path");
            }

            if (fullFile == fullDirectory) { return "/"; }

            // The +1 is to avoid the directory separator
            return fullFile.Substring(fullDirectory.Length + 1).Replace("\\", "/");
        }

        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
    }
}

