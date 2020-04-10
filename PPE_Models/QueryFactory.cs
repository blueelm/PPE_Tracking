using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace PPE_Models
{
   public class QueryFactory
    {
      
        public static string SqlQuery
        {
            get
            {
                var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
                using (var reader = embeddedProvider.GetFileInfo("sql_query.txt").CreateReadStream())
                {
                    using (var stReader = new StreamReader(reader))
                    {
                        return stReader.ReadToEnd();
                    }

                }
            }
        }

       public static string OgQuery
        {
            get
            {
                var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
                using (var reader = embeddedProvider.GetFileInfo("og_query.txt").CreateReadStream())
                {
                    using (var stReader = new StreamReader(reader))
                    {
                        return stReader.ReadToEnd();
                    }

                }
            }
        }

    }
}
