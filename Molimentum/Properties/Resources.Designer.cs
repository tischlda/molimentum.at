﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18034
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Molimentum.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Molimentum.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
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
        ///   Looks up a localized string similar to Name muss angegeben werden..
        /// </summary>
        internal static string AuthorRequired {
            get {
                return ResourceManager.GetString("AuthorRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Text muss angegeben werden..
        /// </summary>
        internal static string BodyRequired {
            get {
                return ResourceManager.GetString("BodyRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ungültiges E-Mail-Adressformat..
        /// </summary>
        internal static string EmailNotValid {
            get {
                return ResourceManager.GetString("EmailNotValid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to E-Mail muss angegeben werden..
        /// </summary>
        internal static string EmailRequired {
            get {
                return ResourceManager.GetString("EmailRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Betreff muss angegeben werden..
        /// </summary>
        internal static string TitleRequired {
            get {
                return ResourceManager.GetString("TitleRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ungültiges Url-Format..
        /// </summary>
        internal static string UrlNotValid {
            get {
                return ResourceManager.GetString("UrlNotValid", resourceCulture);
            }
        }
    }
}
