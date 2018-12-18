using System;
using System.Collections.Generic;
using Ducksoft.Mvc.Razor.Sitemap.Utilities;

namespace Ducksoft.Mvc.Razor.Sitemap.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Ducksoft.Mvc.Razor.Sitemap.Models.ISitemapPage" />
    /// <seealso cref="System.Collections.Generic.IEqualityComparer{Ducksoft.Mvc.Razor.Sitemap.Models.ISitemapPage}" />
    public class SitemapPage : ISitemapPage, IEqualityComparer<ISitemapPage>
    {
        /// <summary>
        /// Gets or sets the name of the area.
        /// </summary>
        /// <value>
        /// The name of the area.
        /// </value>
        public string AreaName { get; set; }

        /// <summary>
        /// Gets or sets the name of the page.
        /// </summary>
        /// <value>
        /// The name of the page.
        /// </value>
        public string PageName { get; set; }

        /// <summary>
        /// Gets or sets the relative path.
        /// </summary>
        /// <value>
        /// The relative path.
        /// </value>
        public string RelativePath { get; set; }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the last modified.
        /// </summary>
        /// <value>
        /// The last modified.
        /// </value>
        public DateTime? LastModified { get; set; }

        #region Interface: IEqualityComparer implementation

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(ISitemapPage x, ISitemapPage y)
        {
            //Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            //Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            {
                return false;
            }

            return x.Equals(y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(ISitemapPage obj)
        {
            //Check whether the object is null
            if (ReferenceEquals(obj, null))
            {
                return 0;
            }

            return obj.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            var target = default(ISitemapPage);
            if (obj.GetType().Implements<ISitemapPage>())
            {
                target = (ISitemapPage)obj;
            }

            var isEqual = AreaName.IsEqualTo(target.AreaName) &&
                PageName.IsEqualTo(target.PageName);

            return isEqual;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return (AreaName?.GetHashCode() ?? 0) ^
                (PageName?.GetHashCode() ?? 0);
        }

        #endregion
    }
}
