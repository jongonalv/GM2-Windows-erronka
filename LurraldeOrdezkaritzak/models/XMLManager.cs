using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace lurraldeOrdezkaritzak
{
    public class XmlManager
    {
        public List<Artikuloa> LoadArtikuloakFromXml(string filePath, bool isResource = false)
        {
            try
            {
                Stream stream;

                if (isResource)
                {
                    var assembly = IntrospectionExtensions.GetTypeInfo(typeof(XmlManager)).Assembly;
                    stream = assembly.GetManifestResourceStream(filePath);

                    if (stream == null)
                        throw new FileNotFoundException($"No se encontró el recurso embebido: {filePath}");
                }
                else
                {
                    if (!File.Exists(filePath))
                        throw new FileNotFoundException($"El archivo no existe: {filePath}");

                    stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                }

                using (stream)
                {
                    var xmlSerializer = new XmlSerializer(typeof(List<Artikuloa>), new XmlRootAttribute("Artikuloak"));
                    return (List<Artikuloa>)xmlSerializer.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar el archivo XML: {ex.Message}");
                return new List<Artikuloa>();
            }
        }


        public void SaveArtikuloakToXml(List<Artikuloa> artikuloak, string filePath)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(List<Artikuloa>), new XmlRootAttribute("Artikuloak"));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    xmlSerializer.Serialize(stream, artikuloak);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar el archivo XML: {ex.Message}");
            }
        }
    }
}
