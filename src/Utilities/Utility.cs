using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ducksoft.Mvc.Razor.Sitemap.Utilities
{
    /// <summary>
    /// Static class which is used to perform common functionality across the application.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Gets the directory path.
        /// </summary>
        /// <param name="fileFullPath">The file full path.</param>
        /// <returns>System.String.</returns>
        public static string GetDirectoryPath(string fileFullPath) =>
            Path.GetDirectoryName(fileFullPath);

        /// <summary>
        /// Gets the file name without extension.
        /// </summary>
        /// <param name="fileFullPath">The file full path.</param>
        /// <returns>System.String.</returns>
        public static string GetFileNameWithoutExtension(string fileFullPath) =>
            Path.GetFileNameWithoutExtension(fileFullPath);

        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <param name="combinedPath">The combined path.</param>
        /// <returns></returns>
        public static string GetFullPath(string combinedPath) => Path.GetFullPath(combinedPath);

        /// <summary>
        /// Gets the combined path.
        /// </summary>
        /// <param name="parentPath">The parent path.</param>
        /// <param name="childPath">The child path.</param>
        /// <returns>System.String.</returns>
        public static string GetCombinedPath(string parentPath, string childPath) =>
            GetCombinedPath(new string[] { parentPath, childPath });

        /// <summary>
        /// Gets the combined path.
        /// </summary>
        /// <param name="paths">The list of paths to combine.</param>
        /// <returns>System.String.</returns>
        public static string GetCombinedPath(params string[] paths)
        {
            var filePaths = paths.ToList();
            if (filePaths.Count > 1)
            {
                var childPaths = filePaths.Skip(1).Select(P => P.GetValidChildPath());
                filePaths = filePaths.Take(1).Concat(childPaths).ToList();
            }

            return GetFullPath(Path.Combine(filePaths.ToArray()));
        }

        /// <summary>
        /// Gets the valid child path.
        /// </summary>
        /// <param name="childPath">The child path.</param>
        /// <returns></returns>
        public static string GetValidChildPath(this string childPath)
        {
            //Hp --> BugFix: Path.Combine fails to return file full path if child path starts with
            //directory separator.
            if (Path.IsPathRooted(childPath))
            {
                var delimeters = new char[]
                {
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar
                };

                childPath = childPath.TrimStart(delimeters);
            }

            return childPath;
        }

        /// <summary>
        /// Determines whether [is file exists] [the specified file path].
        /// </summary>
        /// <param name="fileFullPath">The file full path.</param>
        /// <returns><c>true</c> if [is file exists] [the specified file path]; 
        /// otherwise, <c>false</c>.</returns>
        public static bool IsFileExists(string fileFullPath) => File.Exists(fileFullPath);

        /// <summary>
        /// To the URL relative path.
        /// </summary>
        /// <param name="fileRelativePath">The file relative path.</param>
        /// <returns></returns>
        public static string ToUrlRelativePath(this string fileRelativePath)
        {
            var relativePath = string.Empty;
            if (string.IsNullOrWhiteSpace(fileRelativePath))
            {
                return relativePath;
            }

            return fileRelativePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        /// <summary>
        /// Gets the last modified date.
        /// </summary>
        /// <param name="fileFullPath">The file full path.</param>
        /// <returns></returns>
        public static DateTime? GetLastModifiedDate(string fileFullPath)
        {
            var lastModified = (DateTime?)null;
            try
            {
                if (IsFileExists(fileFullPath))
                {
                    lastModified = new FileInfo(fileFullPath).LastWriteTime;
                }
            }
            catch
            {
                //Hp --> Do nothing, jusr return current date time.
            }

            return lastModified;
        }

        /// <summary>
        /// Determines whether [is equal to] [the specified source].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>
        ///   <c>true</c> if [is equal to] [the specified source]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEqualTo(this string source, string target,
            StringComparison comparer = StringComparison.InvariantCultureIgnoreCase)
        {
            string trimData(string data) => (data != null) ? data.Trim() : data;
            return string.Equals(trimData(source), trimData(target), comparer);
        }

        /// <summary>
        /// Implementses the specified type.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Only interfaces can be implemented.</exception>
        public static bool Implements<TInterface>(this Type type) where TInterface : class
        {
            var interfaceType = typeof(TInterface);
            if (!interfaceType.IsInterface)
            {
                throw new InvalidOperationException(
                    "The given type does not implements provided interface.");
            }

            return interfaceType.IsAssignableFrom(type);
        }

        /// <summary>
        /// Gets the name of the page.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetPageName<T>() where T : PageModel => GetPageName(typeof(T));

        /// <summary>
        /// Gets the name of the page.
        /// </summary>
        /// <param name="pageModelType">Type of the page model.</param>
        /// <returns></returns>
        public static string GetPageName(this Type pageModelType) =>
            (pageModelType?.IsSubclassOf(typeof(PageModel)) ?? false) ?
            GetPageName(pageModelType.Name) : string.Empty;

        /// <summary>
        /// Gets the name of the page.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <returns></returns>
        public static string GetPageName(string modelName)
        {
            var pageName = modelName?.Replace("Model", string.Empty)?.Trim() ?? string.Empty;
            return string.IsNullOrWhiteSpace(pageName) ? string.Empty : $"/{pageName}";
        }

        /// <summary>
        /// Gets the name of the area.
        /// </summary>
        /// <param name="pageModelType">Type of the page model.</param>
        /// <returns></returns>
        public static string GetAreaName(this Type pageModelType)
        {
            var areaName = string.Empty;
            if (!pageModelType?.IsSubclassOf(typeof(PageModel)) ?? true)
            {
                return areaName;
            }

            var delimeter = new char[] { '.' };
            var assemblyName = pageModelType.Assembly.GetName().Name;
            var classNamespace = pageModelType.Namespace;
            areaName = classNamespace
                .Replace(assemblyName, string.Empty)
                .Replace("Areas", string.Empty)
                .Split(delimeter, StringSplitOptions.RemoveEmptyEntries)
                .Take(1)
                .Single();

            return areaName;
        }

        /// <summary>
        /// Serializes to XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcObject">The source object.</param>
        /// <param name="isWcfObject">if set to <c>true</c> [is WCF object].</param>
        /// <returns></returns>
        public static string SerializeToXml<T>(this T srcObject, bool isWcfObject = false)
            where T : class
        {
            var strBuilder = new StringBuilder();
            using (var writer = XmlWriter.Create(strBuilder))
            {
                if (isWcfObject)
                {
                    var serializer = new DataContractSerializer(typeof(T));
                    serializer.WriteObject(writer, srcObject);
                }
                else
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(writer, srcObject);
                }

                writer.Flush();
            }

            return strBuilder.ToString();
        }
    }
}
