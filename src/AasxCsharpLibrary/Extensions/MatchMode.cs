/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Extensions
{
    public enum MatchMode
    {
        Strict,             //may be not needed in future, as no local flag in V3
        Relaxed,            //should be as default
        RelaxedIgnoreCase,  // more relax, e.g. for URI matching
        Identification,     // only id
        IdNoVersion         // without version
    }

    public class MatchModeUtil
    {

        /// <summary>
        /// Removes the version information
        /// from URI/IRI and IRDI.
        /// </summary>
        public static string IriIrdiRemoveVersion(string id)
        {
            // access
            if (id == null)
                return null;

            // IRDIs(?) -> can an IRDI Path identified?
            if (Regex.IsMatch(id, @"^((\d{4,4}).*/)"))
            {
                // assume separation char
                var sepChar = '/';

                // IRDI path
                var path = id.Split(sepChar);
                if (path != null && path.Length >= 0)
                {
                    // ok, promising
                    var newPath = new List<string>();

                    foreach (var pp in path)
                    {
                        var ppid = pp;
                        var m2 = Regex.Match(ppid, @"((\d{4,4}).*)(#\d{2,4})");
                        if (m2.Success && m2.Groups.Count >= 1)
                            ppid = m2.Groups[1].ToString();
                        newPath.Add(ppid);
                    }

                    // ok, return that!
                    return string.Join(sepChar, newPath);
                }
            }

            // No .. ordinary approach
            // if id starts with 4 digits, it is an IRDI
            var m = Regex.Match(id, @"((\d{4,4}).*([A-Za-z0-9]{4,6}))(#\d{2,4})");
            if (m.Success && m.Groups.Count >= 1)
                id = m.Groups[1].ToString();

            // if id starts with 'http', it is an IRDI
            m = Regex.Match(id, @"((http:).*?)(/\d{1,4}){1,2}");
            if (m.Success && m.Groups.Count >= 1)
                id = m.Groups[1].ToString();

            return id;
        }

        /// <summary>
        /// If required by match mode, removes the version information
        /// from URI/IRI and IRDI.
        /// </summary>
        public static string FilterId(string id, MatchMode mm)
        {
            if (mm == MatchMode.IdNoVersion)
                return IriIrdiRemoveVersion(id);
            return id;
        }

    }
}
