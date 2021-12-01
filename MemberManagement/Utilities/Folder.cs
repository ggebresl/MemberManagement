using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberManagement.Utilities
{
    public class Folder
    {
        public string FolderType { get; set; }

        public string GetFoldeType(string folderType)
        {
            string returnVal = string.Empty;

            switch (folderType)
            {
                case "audio":
                    returnVal = folderType;
                    break;
                case "doc":
                    returnVal = folderType;
                    break;

                case "image":
                    returnVal = folderType;
                    break;
                case "video":
                    returnVal = folderType;
                    break;

                default:
                    returnVal = folderType;
                    break;
            }

            return returnVal;
        }
    }
}
    