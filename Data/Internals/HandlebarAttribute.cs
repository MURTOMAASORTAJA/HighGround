using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Internals
{
    /// <summary>
    /// A property attribute exposing the property to be referenced with a {{ handlebar-name }} in a web page.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class HandlebarAttribute : Attribute
    {
        private string[] names;

        public string Name { 
            get => names.Any() ? names.First() : ""; 
            set {
                ValidateName(value);
                names = new[] { value };
            }}

        public string[] Names
        {
            get
            {
                return names;
            }

            set
            {
                if (value.Any())
                {
                    foreach (var val in value)
                    {
                        ValidateName(val);
                    }
                } else
                {
                    names = Array.Empty<string>();
                }
            }
        }

        public HandlebarAttribute(string name)
        {
            Name = name;
        }
        public HandlebarAttribute(string[] names) => this.names = names;

        protected static void ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("HandlebarAttribute name can't be null or empty.");
            }

            if (name.Contains(" "))
            {
                throw new ArgumentException("HandlebarAttribute name can't contain \" \".");
            }
        }
    }
}
