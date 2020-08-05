using DocFxTocGenerate;
using System.IO;

namespace DocFxMarkdownExtractor
{
    public class FileUtility
    {
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }

        public FileUtility(string sourceDirectory, string targetDirectory)
        {
            this.SourceDirectory = sourceDirectory;
            this.TargetDirectory = targetDirectory;
        }

        /// <summary>
        /// Recursively walk the SourceDirectory looking for markdown files, and output a TOC.yml file to TargetDirectory.
        /// </summary>
        public void WalkPath()
        {
            if (!Directory.Exists(this.TargetDirectory))
            {
                Directory.CreateDirectory(this.TargetDirectory);
            }
            var tocFile = Path.Combine(this.TargetDirectory, "toc.yml");

            using (StreamWriter tocFileStream = new StreamWriter(tocFile))
            {
                foreach (var sourceFile in Directory.EnumerateFiles(this.SourceDirectory, "*.md", SearchOption.AllDirectories))
                {
                    var fileModel = this.BuildFileModel(sourceFile);

                    if (!Directory.Exists(fileModel.FullTargetDir))
                    {
                        Directory.CreateDirectory(fileModel.FullTargetDir);
                    }
                    if (!File.Exists(fileModel.TargetFile))
                    {
                        File.Copy(sourceFile, fileModel.TargetFile);
                    }

                    // Heading entry:
                    tocFileStream.WriteLine((string.IsNullOrEmpty(fileModel.SourceDirName)) ? $"- name: {fileModel.SourceFileName}" : $"- name: {fileModel.SourceFileName} in {fileModel.SourceDirName}");
                    // Path entry:
                    tocFileStream.WriteLine($"  href: {fileModel.SourceRelativeFile}");
                }
            }
        }

        /// <summary>
        /// Given SourceDirectory, the current file, and TargetDirectory, build a FileModel object.
        /// </summary>
        public FileModel BuildFileModel(string sourceFile)
        {
            var fileName = Path.GetFileName(sourceFile);
            var relativeFile = Path.GetRelativePath(this.SourceDirectory, sourceFile);
            var dirName = Path.GetDirectoryName(relativeFile);

            var fullTargetDir = Path.Combine(this.TargetDirectory, dirName);
            var targetFile = Path.Combine(fullTargetDir, fileName);

            var fileModel = new FileModel()
            {
                SourceFileName = fileName,
                SourceRelativeFile = relativeFile,
                SourceDirName = dirName,
                FullTargetDir = fullTargetDir,
                TargetFile = targetFile
            };

            return fileModel;
        }
    }
}
