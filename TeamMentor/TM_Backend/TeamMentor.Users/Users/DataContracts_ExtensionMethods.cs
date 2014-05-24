using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
    public static class DataContracts_ExtensionMethods
    {        
        public static List<ValidationResult>           validate             (this object objectTovalidate)
        {
            var results = new List<ValidationResult>();
            if (objectTovalidate.notNull())
            {
                var context = new ValidationContext(objectTovalidate, null, null);
                Validator.TryValidateObject(objectTovalidate, context, results, true);
                if (results.size()>0)
                    TM_UserData.Current.logTBotActivity("User Validation Failed",results.asStringList().asString());
            }            
            return results;
        }
        public static bool                             validation_Ok        (this object objectTovalidate)
        {
            return objectTovalidate.validate().empty();
        }
        public static bool                             validation_Failed    (this object objectTovalidate)
        {
            return objectTovalidate.validate().notEmpty();
        }
        public static List<string> asStringList(this List<ValidationResult> validationResults)
        {
            return (from validationResult in validationResults
                from memberName in validationResult.MemberNames
                select "{0}:{1}".format(memberName,validationResult.ErrorMessage)).toList();
        }
        public static Dictionary<string, List<string>> indexed_By_MemberName(this List<ValidationResult> validationResults)
        {
            var mappedData = new Dictionary<string, List<string>>();
            foreach(var validationResult in validationResults)
                foreach (var memberName in validationResult.MemberNames)
                    mappedData.add(memberName, validationResult.ErrorMessage);  
            return mappedData;
        }
    }
}