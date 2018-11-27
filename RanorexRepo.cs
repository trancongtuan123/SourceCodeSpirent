using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;




namespace RxAgent
{

    [System.CodeDom.Compiler.GeneratedCode("Ranorex", "5.1.2")]
    [RepositoryFolder("3fc036d9-02f9-4d8a-8b14-1ab3058a3e74")]
    public partial class testRepository : RepoGenBaseFolder
    {
        static testRepository instance = new testRepository();
        testRepositoryFolders.InnerFolder _innerFolder;

        /// <summary>
        /// Gets the singleton class instance representing the testRepository element repository.
        /// </summary>
        [RepositoryFolder("3fc036d9-02f9-4d8a-8b14-1ab3058a3e74")]
        public static testRepository Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Repository class constructor.
        /// </summary>
        public testRepository()
            : base("Test Page", "/", null, 0, false, "3fc036d9-02f9-4d8a-8b14-1ab3058a3e74", ".\\RepositoryImages\\testRepository3fc036d9.rximgres")
        {
            _innerFolder = new testRepositoryFolders.InnerFolder(this);
        }

        #region Variables

        #endregion

        /// <summary>
        /// The Self item info.
        /// </summary>
        [RepositoryItemInfo("3fc036d9-02f9-4d8a-8b14-1ab3058a3e74")]
        public virtual RepoItemInfo SelfInfo
        {
            get
            {
                return _selfInfo;
            }
        }

        /// <summary>
        /// The FlashSampleMozillaFirefox folder.
        /// </summary>
        [RepositoryFolder("f6a0d3dc-69fc-4663-be86-24c0372a4398")]
        public virtual testRepositoryFolders.InnerFolder InnerFolder
        {
            get { return _innerFolder; }
        }

    }

    /// <summary>
    /// Inner folder classes.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("Ranorex", "5.1.2")]
    public partial class testRepositoryFolders
    {
        /// <summary>
        /// The FlashSampleMozillaFirefoxAppFolder folder.
        /// </summary>
        [RepositoryFolder("f6a0d3dc-69fc-4663-be86-24c0372a4398")]
        public partial class InnerFolder : RepoGenBaseFolder
        {

            /// <summary>
            /// Creates a new FlashSampleMozillaFirefox  folder.
            /// </summary>
            public InnerFolder(RepoGenBaseFolder parentFolder) :
                base("InnerFolder", "", parentFolder, 30000, null, true, "f6a0d3dc-69fc-4663-be86-24c0372a4398", "")
            {
              
            }

 
        }

        

    }
}