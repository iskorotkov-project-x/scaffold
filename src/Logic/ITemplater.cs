using Model;

namespace Business
{
    /// <summary>
    /// Templater replaces templates in files
    /// </summary>
    public interface ITemplater
    {
        /// <summary>
        /// Replace templates which are specified in ctx in the project
        /// </summary>
        /// <param name="ctx">Context in which replace happend</param>
        /// <param name="project">The project that is being replaced</param>
        /// <returns></returns>
        Project Substitute(Context ctx, Project project);
    }
}
