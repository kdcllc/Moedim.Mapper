using System.CommandLine;
using Moedim.Mapper.Migration.Tool;

var rootCommand = new RootCommand("Moedim.Mapper Migration Tool - Migrate from AutoMapper to Moedim.Mapper");

// Analyze command
var analyzePathArg = new Argument<string>("path", "Path to project or solution file");
var analyzeVerboseOpt = new Option<bool>(new[] { "--verbose", "-v" }, "Show detailed analysis");
var analyzeOutputOpt = new Option<string?>(new[] { "--output", "-o" }, "Output file for analysis report");

var analyzeCommand = new Command("analyze", "Analyze project for AutoMapper usage")
{
    analyzePathArg,
    analyzeVerboseOpt,
    analyzeOutputOpt
};

analyzeCommand.SetHandler(async (string path, bool verbose, string? output) =>
{
    await MigrationAnalyzer.AnalyzeAsync(path, verbose, output);
}, analyzePathArg, analyzeVerboseOpt, analyzeOutputOpt);

// Migrate command
var migratePathArg = new Argument<string>("path", "Path to project or solution file");
var migrateStrategyOpt = new Option<string>(new[] { "--strategy", "-s" }, () => "attributes", "Migration strategy: 'attributes' or 'fluent'");
var migrateDryRunOpt = new Option<bool>(new[] { "--dry-run", "-d" }, "Show changes without applying them");
var migrateBackupOpt = new Option<bool>(new[] { "--backup", "-b" }, () => true, "Create backup before migration");

var migrateCommand = new Command("migrate", "Migrate AutoMapper code to Moedim.Mapper")
{
    migratePathArg,
    migrateStrategyOpt,
    migrateDryRunOpt,
    migrateBackupOpt
};

migrateCommand.SetHandler(async (string path, string strategy, bool dryRun, bool backup) =>
{
    await CodeMigrator.MigrateAsync(path, strategy, dryRun, backup);
}, migratePathArg, migrateStrategyOpt, migrateDryRunOpt, migrateBackupOpt);

// Report command
var reportPathArg = new Argument<string>("path", "Path to project or solution file");
var reportFormatOpt = new Option<string>(new[] { "--format", "-f" }, () => "markdown", "Report format: 'markdown', 'json', or 'html'");
var reportOutputOpt = new Option<string?>(new[] { "--output", "-o" }, "Output file for report");

var reportCommand = new Command("report", "Generate migration compatibility report")
{
    reportPathArg,
    reportFormatOpt,
    reportOutputOpt
};

reportCommand.SetHandler(async (string path, string format, string? output) =>
{
    await ReportGenerator.GenerateAsync(path, format, output);
}, reportPathArg, reportFormatOpt, reportOutputOpt);

rootCommand.AddCommand(analyzeCommand);
rootCommand.AddCommand(migrateCommand);
rootCommand.AddCommand(reportCommand);

return await rootCommand.InvokeAsync(args);
