﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.225
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace SLFileUpload {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource1 {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource1() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SLFileUpload.Resource1", typeof(Resource1).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   使用此强类型资源类，为所有资源查找
        ///   重写当前线程的 CurrentUICulture 属性。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 已取消 的本地化字符串。
        /// </summary>
        internal static string FileUploadStatus_Canceled {
            get {
                return ResourceManager.GetString("FileUploadStatus_Canceled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 完成 的本地化字符串。
        /// </summary>
        internal static string FileUploadStatus_Complete {
            get {
                return ResourceManager.GetString("FileUploadStatus_Complete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 错误 的本地化字符串。
        /// </summary>
        internal static string FileUploadStatus_Error {
            get {
                return ResourceManager.GetString("FileUploadStatus_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 解析文件... 的本地化字符串。
        /// </summary>
        internal static string FileUploadStatus_Parsing {
            get {
                return ResourceManager.GetString("FileUploadStatus_Parsing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 就绪 的本地化字符串。
        /// </summary>
        internal static string FileUploadStatus_Pending {
            get {
                return ResourceManager.GetString("FileUploadStatus_Pending", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 已移除 的本地化字符串。
        /// </summary>
        internal static string FileUploadStatus_Removed {
            get {
                return ResourceManager.GetString("FileUploadStatus_Removed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 调整大小... 的本地化字符串。
        /// </summary>
        internal static string FileUploadStatus_Resizing {
            get {
                return ResourceManager.GetString("FileUploadStatus_Resizing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 正在上传... 的本地化字符串。
        /// </summary>
        internal static string FileUploadStatus_Uploading {
            get {
                return ResourceManager.GetString("FileUploadStatus_Uploading", resourceCulture);
            }
        }
    }
}
