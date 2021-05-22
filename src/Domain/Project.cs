using System.Collections.Generic;

namespace Model
{
    /// <summary>
    /// Information about created project
    /// </summary>
    public class Project
    {
        public IEnumerable<File> CreatedFiles { get; init; }
        public IEnumerable<File> ProcessedFiles { get; init; }
        public bool Compiles()
        {
            throw new System.NotImplementedException();
        }
    }
}
