using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ikaslea.KomertzialakApp.Models.Enums
{
    public enum Egoera
    {
        BIDALITA,
        PRESTATUTA,
        PRESTATZEN,
        BUKATUTA
    }

    public static class EgoeraExtensions
    {
        public static Egoera FromString(string text)
        {
            foreach (Egoera e in Enum.GetValues(typeof(Egoera)))
            {
                if (e.ToString().Equals(text, StringComparison.OrdinalIgnoreCase))
                {
                    return e;
                }
            }
            throw new ArgumentException($"No enum constant {typeof(Egoera).FullName} with text {text}");
        }
    }
}

