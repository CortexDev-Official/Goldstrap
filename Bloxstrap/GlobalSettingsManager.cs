using Bloxstrap.Enums.FlagPresets;
using Bloxstrap.Enums.GBSPresets;
using Microsoft.VisualBasic;
using System.ComponentModel.Design.Serialization;

using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
namespace Bloxstrap
{
    public class GlobalSettingsManager
    {
        public XDocument? Document { get; set; } = null!;

        public Dictionary<string, string> PresetPaths = new()
        {
            { "Rendering.FramerateCap", "{UserSettings}/int[@name='FramerateCap']" },
            { "Rendering.SavedQualityLevel", "{UserSettings}/token[@name='SavedQualityLevel']" }, 

            
            
            { "Rendering.GraphicsQualityLevel", "{UserSettings}/int[@name='GraphicsQualityLevel']" }, 
            { "Rendering.MaxQualityEnabled", "{UserSettings}/bool[@name='MaxQualityEnabled']" },

            { "User.MouseSensitivity", "{UserSettings}/float[@name='MouseSensitivity']"},
            { "User.VREnabled", "{UserSettings}/bool[@name='VREnabled']"},

            
            { "UI.Transparency", "{UserSettings}/float[@name='PreferredTransparency']" },
            { "UI.ReducedMotion", "{UserSettings}/bool[@name='ReducedMotion']" },
            { "UI.FontSize", "{UserSettings}/token[@name='PreferredTextSize']" }
        };

        
        
        
        public Dictionary<string, string> RootPaths = new()
        {
            { "UserSettings", "//Item[@class='UserGameSettings']/Properties" },
        };

        public static IReadOnlyDictionary<FontSize, string?> FontSizes => new Dictionary<FontSize, string?>
        {
            { FontSize.x1, "1" },
            { FontSize.x2, "2" },
            { FontSize.x3, "3" },
            { FontSize.x4, "4" }
        };

        public bool Loaded { get; set; } = false;

        public string FileLocation => Path.Combine(Paths.Roblox, "GlobalBasicSettings_13.xml");

        public void SetPreset(string prefix, object? value)
        {
            foreach (var pair in PresetPaths.Where(x => x.Key.StartsWith(prefix)))
                SetValue(pair.Value, value);
        }

        public string? GetPreset(string prefix)
        {
            if (!PresetPaths.ContainsKey(prefix))
                return null;

            return GetValue(PresetPaths[prefix]);
        }

        /// <summary>
        /// Values changed by the user since the last save, keyed by their resolved xpath.
        /// Only these get written back to disk, so that settings roblox itself changed
        /// while Goldstrap was open (graphics quality, volume, etc) don't get reverted.
        /// </summary>
        private readonly Dictionary<string, string> _pendingChanges = new();

        public bool Changed => _pendingChanges.Count > 0;

        public void SetValue(string path, object? value)
        {
            if (value is null)
                return;

            path = ResolvePath(path);

            string stringValue = value is bool boolean
                ? boolean.ToString().ToLowerInvariant() 
                : Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;

            if (!ApplyValue(path, stringValue))
                return;

            _pendingChanges[path] = stringValue;
        }

        /// <summary>
        /// Writes a value into the in-memory document, creating the element if it doesn't exist yet.
        /// Returns false if the document isn't loaded or the path can't be created.
        /// </summary>
        private bool ApplyValue(string resolvedPath, string value)
        {
            const string LOG_IDENT = "GBSEditor::ApplyValue";

            if (Document is null)
                return false;

            try
            {
                XElement? element = Document.XPathSelectElement(resolvedPath);

                if (element is null)
                {
                    
                    
                    var match = Regex.Match(resolvedPath, @"^(?<parent>.*)/(?<type>[A-Za-z0-9_]+)\[@name='(?<name>[^']+)'\]$");

                    if (!match.Success)
                        return false;

                    XElement? parent = Document.XPathSelectElement(match.Groups["parent"].Value);

                    if (parent is null)
                        return false;

                    element = new XElement(match.Groups["type"].Value, new XAttribute("name", match.Groups["name"].Value));
                    parent.Add(element);
                }

                element.Value = value;

                return true;
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, $"Failed to apply value at {resolvedPath}");
                App.Logger.WriteException(LOG_IDENT, ex);

                return false;
            }
        }

        public string? GetValue(string path)
        {
            path = ResolvePath(path);

            return Document?.XPathSelectElement(path)?.Value;
        }

        public bool previousReadOnlyState;

        public void SetReadOnly(bool readOnly, bool preserveState = false)
        {
            const string LOG_IDENT = "GBSEditor::SetReadOnly";

            if (!File.Exists(FileLocation))
                return;

            try
            {
                FileAttributes attributes = File.GetAttributes(FileLocation);

                if (readOnly)
                    attributes |= FileAttributes.ReadOnly;
                else
                    attributes &= ~FileAttributes.ReadOnly;

                File.SetAttributes(FileLocation, attributes);

                if (!preserveState)
                    previousReadOnlyState = readOnly;
            } catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, $"Failed to set read-only on {FileLocation}");
                App.Logger.WriteException(LOG_IDENT, ex);
            }
        }

        public bool GetReadOnly()
        {
            if (!File.Exists(FileLocation))
                return false;

            return File.GetAttributes(FileLocation).HasFlag(FileAttributes.ReadOnly);
        }

        public void CreateTemplate()
        {
            string LOG_IDENT = "GBSEditor::CreateTemplate";
            App.Logger.WriteLine(LOG_IDENT, $"Creating template at {FileLocation}...");

            try
            {
                var uri = new Uri("pack://application:,,,/Resources/GlobalBasicSettings_Template.xml");
                var resourceInfo = Application.GetResourceStream(uri);

                using (Stream resourceStream = resourceInfo.Stream)
                using (FileStream fileStream = File.Create(FileLocation))
                {
                    resourceStream.CopyTo(fileStream);
                }

                previousReadOnlyState = GetReadOnly();
            } catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, $"Failed to create template at {FileLocation}");
                App.Logger.WriteException(LOG_IDENT, ex);
            }
        }

        public void Load()
        {
            string LOG_IDENT = "GBSEditor::Load";

            App.Logger.WriteLine(LOG_IDENT, $"Loading from {FileLocation}...");

            
            
            if (!File.Exists(FileLocation))
                CreateTemplate();

            _pendingChanges.Clear();

            if (ReloadDocument())
                Loaded = true;
            else
                App.Logger.WriteLine(LOG_IDENT, "Failed to load!");
        }

        /// <summary>
        /// Re-reads the document from disk, but only when there's nothing unsaved to lose.
        /// </summary>
        public void RefreshIfUnchanged()
        {
            if (!Loaded || Changed)
                return;

            ReloadDocument();
        }

        /// <summary>
        /// Re-reads the document from disk, discarding any unapplied in-memory state.
        /// Returns false only if the file exists but couldn't be read.
        /// </summary>
        private bool ReloadDocument()
        {
            const string LOG_IDENT = "GBSEditor::ReloadDocument";

            
            if (!File.Exists(FileLocation))
                return Document is not null;

            try
            {
                using var reader = XmlReader.Create(FileLocation, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit });

                Document = XDocument.Load(reader);
                previousReadOnlyState = GetReadOnly();

                return true;
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, $"Failed to read {FileLocation}");
                App.Logger.WriteException(LOG_IDENT, ex);

                return false;
            }
        }

        public virtual void Save()
        {
            string LOG_IDENT = "GBSEditor::Save";

            if (!Loaded)
                return;

            if (!Changed)
            {
                App.Logger.WriteLine(LOG_IDENT, "Nothing changed, not saving");
                return;
            }

            App.Logger.WriteLine(LOG_IDENT, $"Saving to {FileLocation}...");

            try
            {
                
                
                
                if (!ReloadDocument())
                {
                    App.Logger.WriteLine(LOG_IDENT, "Failed to re-read the file, aborting save to avoid overwriting it");
                    return;
                }

                foreach (var pair in _pendingChanges)
                    ApplyValue(pair.Key, pair.Value);

                SetReadOnly(false, true);
                Document?.Save(FileLocation);

                SetReadOnly(previousReadOnlyState);

                _pendingChanges.Clear();
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, "Failed to save");
                App.Logger.WriteException(LOG_IDENT, ex);

                return;
            }

            App.Logger.WriteLine(LOG_IDENT, "Save complete!");
        }

        private string ResolvePath(string rawPath)
        {
            return Regex.Replace(rawPath, @"\{(.+?)\}", match =>
            {
                string key = match.Groups[1].Value;
                return RootPaths.TryGetValue(key, out var value) ? value : match.Value; ;
            });
        }
    }
}