using System;
using System.Linq;

namespace RabbitOperations.Domain
{
    public class TypeName : IEquatable<TypeName>
    {
        public TypeName()
        {
        }

        public TypeName(string fullName)
        {
            var parts = fullName.Split(',');
            ParseClassAndNamespace(parts);
            ParseAssemblyName(parts);
            ParseVersion(parts);
            ParseCulture(parts);
            ParsePublicKeyToken(parts);
        }

        public string ClassName { get; set; }
        public string Namespace { get; set; }
        public string Assembly { get; set; }
        public Version Version { get; set; }
        public string Culture { get; set; }
        public string PublicKeyToken { get; set; }

        public bool Equals(TypeName other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(PublicKeyToken, other.PublicKeyToken) && Equals(Version, other.Version) &&
                   string.Equals(Culture, other.Culture) && string.Equals(Assembly, other.Assembly) &&
                   string.Equals(Namespace, other.Namespace) && string.Equals(ClassName, other.ClassName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TypeName) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (PublicKeyToken != null ? PublicKeyToken.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Version != null ? Version.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Culture != null ? Culture.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Assembly != null ? Assembly.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Namespace != null ? Namespace.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ClassName != null ? ClassName.GetHashCode() : 0);
                return hashCode;
            }
        }

        private void ParseCulture(string[] parts)
        {
            if (parts.Length > 3)
            {
                var cultureParts = parts[3].Trim().Split('=');
                if (cultureParts.Length == 2)
                {
                    Culture = cultureParts[1];
                }
            }
        }

        private void ParsePublicKeyToken(string[] parts)
        {
            if (parts.Length > 4)
            {
                var keyTokenParts = parts[4].Trim().Split('=');
                if (keyTokenParts.Length == 2)
                {
                    PublicKeyToken = keyTokenParts[1];
                }
            }
        }

        private void ParseVersion(string[] parts)
        {
            if (parts.Length > 2)
            {
                var versionParts = parts[2].Trim().Split('=');
                if (versionParts.Length == 2)
                {
                    Version = new Version(versionParts[1]);
                }
            }
        }

        private void ParseAssemblyName(string[] parts)
        {
            if (parts.Length > 1)
            {
                Assembly = parts[1].Trim();
            }
        }

        private void ParseClassAndNamespace(string[] parts)
        {
            if (parts.Length > 0)
            {
                var classParts = parts[0].Trim().Split('.');
                ClassName = classParts.Last();
                Namespace = string.Join(".", classParts.Take(classParts.Length - 1));
            }
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}, {2}, Version={3}, Culture={4}, PublicKeyToken={5}", Namespace, ClassName,
                Assembly, Version, Culture, PublicKeyToken);
        }
    }
}