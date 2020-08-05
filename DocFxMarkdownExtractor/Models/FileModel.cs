namespace DocFxTocGenerate
{
	public class FileModel
	{
		/// <summary>
		/// Source file with no directory information.  For example, "..\input\sub1\child1.md" would have a value of "child1.md"
		/// </summary>
		public string SourceFileName { get; set; }

		/// <summary>
		/// Source file with relative path and file name.  For example, "..\input\sub1\child1.md" would have a value of "sub1\child1.md"
		/// </summary>
		public string SourceRelativeFile { get; set; }

		/// <summary>
		/// Relative directory of source file.  For example, "..\input\sub1\child1.md" would have a value of "sub1"
		/// </summary>
		public string SourceDirName { get; set; }

		/// <summary>
		/// Full target directory of source file.  For example, "..\input\sub1\child1.md", with a target directory of "..\output", would have a value of "..\output\sub1"
		/// </summary>
		public string FullTargetDir { get; set; }

		/// <summary>
		/// Full path of target file.  For example, "..\input\sub1\child1.md", with a target directory of "..\output", would have a value of "..\output\sub1\child1"
		/// </summary>
		/// <value></value>
		public string TargetFile { get; set; }
	}
}