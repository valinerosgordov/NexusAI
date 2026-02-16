namespace NexusAI.Presentation.Properties;

[global::System.Runtime.CompilerServices.CompilerGenerated]
[global::System.CodeDom.Compiler.GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.0.0.0")]
internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase
{
    private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));

    public static Settings Default => defaultInstance;

    [global::System.Configuration.UserScopedSetting]
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.Configuration.DefaultSettingValue("en-US")]
    public string PreferredLanguage
    {
        get => (string)this["PreferredLanguage"];
        set => this["PreferredLanguage"] = value;
    }

    [global::System.Configuration.UserScopedSetting]
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.Configuration.DefaultSettingValue("")]
    public string GeminiApiKey
    {
        get => (string)this["GeminiApiKey"];
        set => this["GeminiApiKey"] = value;
    }
}
