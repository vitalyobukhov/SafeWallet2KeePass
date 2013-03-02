using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace SafeWallet2KeePass
{
    class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                Console.WriteLine("Usage: SafeWallet2KeePass.exe SafeWallet.xml KeePass.xml [ProtectRegex]");
                return 0;
            }
            var srcFilename = args[0];
            var dstFilename = args[1];
            var protectRegex = new Regex(args.Length == 3 ? args[2] : "Password", 
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

            if (!File.Exists(srcFilename))
            {
                Console.WriteLine("File does not exists {0}", srcFilename);
                return 2;
            }


            XElement xSafeWallet;
            try
            {
                xSafeWallet = XElement.Load(srcFilename);
            }
            catch
            {
                Console.WriteLine("Unable to load xml from file {0}", srcFilename);
                return 4313;
            }


            var xKeePassFile = new XElement("KeePassFile");
            var xRoot = new XElement("Root");
            xKeePassFile.Add(xRoot);

            var xRootGroup = new XElement("Group");
            xRoot.Add(xRootGroup);
            var xRootGroupName = new XElement("Name");
            xSafeWallet.Attribute("WalletName").
                With(a => xRootGroupName.Value = a.Value);
            xRootGroup.Add(xRootGroupName);

            var xVault = xSafeWallet.Elements().
                FirstOrDefault(e => e.Attribute("Caption").With(c => c.Value) == "Vault");
            foreach(var xFolder in xVault.Return(x => x.Elements(), new XElement[0]))
            {
                var xGroup = new XElement("Group");
                xRootGroup.Add(xGroup);
                var xGroupName = new XElement("Name");
                xFolder.Attribute("Caption").
                    With(a => xGroupName.Value = a.Value);
                xGroup.Add(xGroupName);

                foreach(var xCard in xFolder.Elements().
                    Where(e => !string.IsNullOrEmpty(e.Attribute("Caption").
                        With(c => c.Value))))
                {
                    var xEntry = new XElement("Entry");
                    xGroup.Add(xEntry);

                    var xTitleString = new XElement("String");
                    xEntry.Add(xTitleString);
                    xTitleString.Add(new XElement("Key", "Title"));
                    xTitleString.Add(new XElement("Value", xCard.Attribute("Caption").Value));

                    foreach (var xField in xCard.Elements().
                        Where(e => !string.IsNullOrEmpty(e.Attribute("Caption").
                        With(c => c.Value)) && !string.IsNullOrEmpty(e.Value)))
                    {
                        var xString = new XElement("String");
                        xEntry.Add(xString);

                        xString.Add(new XElement("Key", xField.Attribute("Caption").Value));
                        var xStringValue = new XElement("Value", xField.Value);
                        if (protectRegex.IsMatch(xField.Attribute("Caption").Value)) 
                            xStringValue.Add(new XAttribute("ProtectInMemory", "True"));
                        xString.Add(xStringValue);
                    }
                }
            }

            try
            {
                xKeePassFile.Save(dstFilename);
                Console.WriteLine("Successfully completed");
            }
            catch
            {
                Console.WriteLine("Unable to save xml to file {0}", dstFilename);
                return 82;
            }


            return 0;
        }
    }
}
