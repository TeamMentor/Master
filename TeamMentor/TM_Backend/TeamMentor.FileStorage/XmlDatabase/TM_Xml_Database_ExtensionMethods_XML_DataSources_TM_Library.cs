using System;
using System.Collections.Generic;
using System.Linq;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class TM_Xml_Database_ExtensionMethods_XML_DataSources_TM_Library
    {
        public static TM_Library        tmLibrary(this TM_Xml_Database tmDatabase, string caption)
        {
            if (caption.isGuid())   // if the value provided is a guid, then 
                return tmDatabase.tmLibrary(caption.guid());

            var tmLibrary = (from library in tmDatabase.tmLibraries()
                             where library.Caption == caption
                             select library).first();
            //if (tmLibrary.isNull())
            //	"[TM_Xml_Database] couldn't find library with caption: {0}".error(caption);
            return tmLibrary;
        }		
        public static TM_Library        tmLibrary(this TM_Xml_Database tmDatabase, Guid libraryId)
        {
            var tmLibrary = (from library in tmDatabase.tmLibraries()
                             where library.Id == libraryId
                             select library).first();
            //if (tmLibrary.isNull())
            //	"[TM_Xml_Database] couldn't find library with id: {0}".error(libraryId);
            return tmLibrary;		
        }
        public static List<TM_Library>  tmLibraries(this TM_Xml_Database tmDatabase)
        {
            var libraries = new List<TM_Library>();
            try
            {
                libraries.AddRange(tmDatabase.xmlDB_GuidanceExplorers()
                    .Select(guidanceExplorer => new TM_Library
                    {
                        Id = guidanceExplorer.library.name.guid(), 
                        Caption = guidanceExplorer.library.caption
                    }));
            }
            catch(Exception ex)
            {
                ex.log();
            }
            return libraries;
        }	
        [EditArticles]  public static TM_Library        new_TmLibrary   (this TM_Xml_Database tmDatabase)
        {
            UserGroup.Editor.demand();
            return tmDatabase.new_TmLibrary("Default_Library_{0}".format(6.randomNumbers()));
        }		
        [EditArticles]  public static TM_Library        new_TmLibrary   (this TM_Xml_Database tmDatabase, string libraryCaption )
        {
            UserGroup.Editor.demand();
            var existingLibrary = tmDatabase.tmLibrary(libraryCaption);
            if (existingLibrary.notNull())
            {
                "[TM_Xml_Database] there was already a library called '{0}' to returning it".debug(libraryCaption);
                return existingLibrary;
            }
            tmDatabase.xmlDB_NewGuidanceExplorer(Guid.NewGuid(), libraryCaption);
            return tmDatabase.tmLibrary(libraryCaption);
        }		
        [EditArticles]  public static TM_Xml_Database   delete_Library  (this TM_Xml_Database tmDatabase, TM_Library library)
        {
            UserGroup.Editor.demand();
            tmDatabase.xmlDB_DeleteGuidanceExplorer(library.Id);
            return tmDatabase;
        }
    }
}