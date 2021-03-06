﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using xsext.models;

namespace xsext
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    PrintInstructions();
                }
                else
                {
                    //main operation
                    switch (args[0].ToLower())
                    {
                        case "dws":
                        case "delete-workspace-services":
                            {
                                DeleteWorkspaceServices(args);
                                break;
                            }
                        case "daws":
                        case "delete-all-workspace-services":
                            {
                                DeleteAllWorkspaceServices(args);
                                break;
                            }
                        case "delete-mta-ops-errors":
                        case "dmoe":
                            {
                                DeleteMtaOpsErrors(args);
                                break;
                            }
                        case "delete-all-mta-ops-errors":
                        case "damoe": 
                            {
                                DeleteAllMtaOpsErrors(args);
                                break;
                            }
                        default:
                            {
                                PrintInstructions();
                                break;
                            }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static List<string> GetWhitelist(string[] args)
        {
            string whitelistParam = string.Empty;
            if (args.Length >= 2)
            {
                whitelistParam = args[1];
                if (whitelistParam.StartsWith("whitelist=["))
                {
                    string whitelistContents = whitelistParam.Replace("whitelist=[", "").Replace("]", "");
                    List<string> whitelist = whitelistContents.Split(',').ToList();
                    return whitelist;
                }
            }

            return new List<string>();
        }

        private static List<string> ValidateDeleteWorkspaceServices(string[] args)
        {
            List<string> returnValue = new List<string>();
            if (args.Length >= 2)
            {
                var firstParam = args[1].Trim();
                if (firstParam.StartsWith("whitelist"))
                {
                    var whitelistRegex = "whitelist=\\[[a-zA-Z0-9\\-_,]*[a-zA-Z0-9\\-_]*\\]";
                    bool match = Regex.IsMatch(firstParam, whitelistRegex);
                    if (!match)
                    {
                        returnValue.Add($"Whitelist invalid: {firstParam}");
                    }
                }
                else
                {
                    returnValue.Add($"Parameter invalid: {firstParam}");

                }
            }

            return returnValue;
        }

        private static void DeleteWorkspaceServices(string[] args)
        {
            List<string> validationErrors = ValidateDeleteWorkspaceServices(args);
            if (validationErrors.Count > 0)
            {
                Console.WriteLine("Error:");
                foreach (var error in validationErrors)
                {
                    Console.WriteLine(error);
                }
            }
            else
            {
                //main operation
                Target target = GetTarget();
                if (target == null || target.User.Length == 0 || target.User == "<not logged in>")
                {
                    Console.WriteLine("Not logged in.");
                    return;
                }

                Console.WriteLine("");
                Console.WriteLine("delete-workspace-services");
                Console.WriteLine("---------------");
                Console.WriteLine(target.APIEndpoint);
                Console.WriteLine(target.User);
                Console.WriteLine(target.Org);
                Console.WriteLine(target.Space);
                Console.WriteLine("");

                List<XSAService> services = GetServices();
                List<string> whitelist = GetWhitelist(args);
                List<XSAService> filteredServices = new List<XSAService>();

                if (whitelist.Count > 0)
                {
                    foreach (var whitename in whitelist)
                    {
                        foreach (var service in services)
                        {
                            if (service.Name.Contains(whitename))
                            {
                                continue;
                            }
                            else
                            {
                                filteredServices.Add(service);
                            }
                        }
                    }
                }
                else
                {
                    filteredServices.AddRange(services);
                }

                filteredServices = filteredServices.Where(x => x.Name.StartsWith(target.User)).ToList();

                Console.WriteLine($"{filteredServices.Count} services found for user {target.User}.");
                Console.WriteLine("");

                foreach (var service in filteredServices)
                {
                    Console.WriteLine(service.Name);
                    string result = DeleteService(service.Name);
                    Console.WriteLine(result);
                }

                Console.WriteLine("");
                Console.WriteLine("delete-workspace-services complete.");

            }
        }

        private static List<string> ValidateDeleteAllWorkspaceServices(string[] args)
        {
            List<string> returnValue = new List<string>();
            if (args.Length >= 2)
            {
                var firstParam = args[1].Trim();
                if (firstParam.StartsWith("whitelist"))
                {
                    var whitelistRegex = "whitelist=\\[[a-zA-Z0-9\\-_,]*[a-zA-Z0-9\\-_]*\\]";
                    bool match = Regex.IsMatch(firstParam, whitelistRegex);
                    if (!match)
                    {
                        returnValue.Add($"Whitelist invalid: {firstParam}");
                    }
                }
                else
                {
                    returnValue.Add($"Parameter invalid: {firstParam}");

                }
            }

            return returnValue;
        }

        private static void DeleteAllWorkspaceServices(string[] args)
        {
            List<string> validationErrors = ValidateDeleteAllWorkspaceServices(args);
            if (validationErrors.Count > 0)
            {
                Console.WriteLine("Error:");
                foreach (var error in validationErrors)
                {
                    Console.WriteLine(error);
                }
            }
            else
            {
                //main operation
                Target target = GetTarget();
                if (target == null || target.User.Length == 0 || target.User == "<not logged in>")
                {
                    Console.WriteLine("Not logged in.");
                    return;
                }

                Console.WriteLine("");
                Console.WriteLine("delete-all-workspace-services");
                Console.WriteLine("---------------");
                Console.WriteLine(target.APIEndpoint);
                Console.WriteLine(target.User);
                Console.WriteLine(target.Org);
                Console.WriteLine(target.Space);
                Console.WriteLine("");

                List<XSAService> services = GetServices();
                List<string> whitelist = GetWhitelist(args);
                List<XSAService> filteredServicesWhitelist = new List<XSAService>();


                if (whitelist.Count > 0)
                {
                    foreach (var whitename in whitelist)
                    {
                        foreach (var service in services)
                        {
                            if (service.Name.Contains(whitename))
                            {
                                continue;
                            }
                            else
                            {
                                filteredServicesWhitelist.Add(service);
                            }
                        }
                    }
                }
                else
                {
                    filteredServicesWhitelist.AddRange(services);
                }

                List<XSAUser> users = GetUsers();
                List<XSAService> filteredServices = new List<XSAService>();
                foreach (var user in users)
                {
                    if (user.User.Length == 0)
                    {
                        //skip if no name returned.
                        continue;
                    }
                    filteredServices.AddRange(filteredServicesWhitelist.Where(x => x.Name.StartsWith(user.User)));
                }

                string userList = string.Empty;
                foreach (var user in users)
                {
                    userList += $",{user.User}";
                }
                userList = userList.TrimStart(',');

                Console.WriteLine($"{filteredServices.Count} services found for users {userList}.");
                Console.WriteLine("");

                foreach (var service in filteredServices)
                {
                    Console.WriteLine(service.Name);
                    string result = DeleteService(service.Name);
                    Console.WriteLine(result);
                }

                Console.WriteLine("");
                Console.WriteLine("delete-all-workspace-services complete.");

            }
        }

        private static void DeleteMtaOpsErrors(string[] args)
        {
            //main operation
            Target target = GetTarget();
            if (target == null || target.User.Length == 0 || target.User == "<not logged in>")
            {
                Console.WriteLine("Not logged in.");
                return;
            }

            Console.WriteLine("");
            Console.WriteLine("delete-mta-ops-errors");
            Console.WriteLine("---------------");
            Console.WriteLine(target.APIEndpoint);
            Console.WriteLine(target.User);
            Console.WriteLine(target.Org);
            Console.WriteLine(target.Space);
            Console.WriteLine("");

            List<XSAMtaOperation> services = GetMtaOps();
            List<XSAMtaOperation> filteredMtaOps = new List<XSAMtaOperation>();

            filteredMtaOps = filteredMtaOps.Where(x => x.StartedBy == target.User && (x.Type == "DEPLOY" || x.Type == "UNDEPLOY") && x.Status == "ERROR").ToList();

            Console.WriteLine($"{filteredMtaOps.Count} mta-ops found for user {target.User} with error status.");
            Console.WriteLine("");

            foreach (var operation in filteredMtaOps)
            {
                Console.WriteLine($"{operation.Id} {operation.Type} {operation.MtaId} {operation.Status} {operation}");
                string result = AbortMtaOp(operation.Id, operation.Type);
                Console.WriteLine(result);
            }

            Console.WriteLine("");
            Console.WriteLine("delete-mta-ops-errors complete.");

        }

        private static void DeleteAllMtaOpsErrors(string[] args)
        {
            //main operation
            Target target = GetTarget();
            if (target == null || target.User.Length == 0 || target.User == "<not logged in>")
            {
                Console.WriteLine("Not logged in.");
                return;
            }

            Console.WriteLine("");
            Console.WriteLine("delete-all-mta-ops-errors");
            Console.WriteLine("---------------");
            Console.WriteLine(target.APIEndpoint);
            Console.WriteLine(target.User);
            Console.WriteLine(target.Org);
            Console.WriteLine(target.Space);
            Console.WriteLine("");

            List<XSAMtaOperation> services = GetMtaOps();
            List<XSAMtaOperation> filteredMtaOps = new List<XSAMtaOperation>();

            filteredMtaOps = filteredMtaOps.Where(x => (x.Type == "DEPLOY" || x.Type == "UNDEPLOY") && x.Status == "ERROR").ToList();

            Console.WriteLine($"{filteredMtaOps.Count} mta-ops found with error status.");
            Console.WriteLine("");

            foreach (var operation in filteredMtaOps)
            {
                Console.WriteLine($"{operation.Id} {operation.Type} {operation.MtaId} {operation.Status} {operation}");
                string result = AbortMtaOp(operation.Id, operation.Type);
                Console.WriteLine(result);
            }

            Console.WriteLine("");
            Console.WriteLine("delete-all-mta-ops-errors complete.");

        }

        private static List<XSAMtaOperation> GetMtaOps()
        {
            string command = "xs mta-ops";
            string cmdResult = RunCommand(command);

            List<XSAMtaOperation> mtaOps = new List<XSAMtaOperation>();
            List<string> cmdResults = cmdResult.Split(Environment.NewLine).ToList();
            List<string> filteredResults = cmdResults.Skip(4).ToList();
            foreach (var result in filteredResults)
            {
                if (result != string.Empty)
                {
                    XSAMtaOperation mtaOp = new XSAMtaOperation();
                    string[] entries = result.Split("  ");
                    mtaOp.Id = entries[0];
                    mtaOp.Type = entries[1];
                    mtaOp.MtaId = entries[2];
                    mtaOp.Status = entries[3];
                    mtaOp.StartedAt = entries[4];
                    mtaOp.StartedBy = entries[5];
                    mtaOps.Add(mtaOp);
                }
            }
            return mtaOps;
        }

        private static List<XSAApp> GetApps()
        {
            string command = "xs apps";
            string cmdResult = RunCommand(command);

            List<XSAApp> apps = new List<XSAApp>();
            List<string> cmdResults = cmdResult.Split(Environment.NewLine).ToList();
            List<string> filteredResults = cmdResults.Skip(6).ToList();
            foreach (var result in filteredResults)
            {
                if (result != string.Empty)
                {
                    XSAApp app = new XSAApp();
                    string[] entries = result.Split(" ");
                    app.Name = entries[0];
                    app.RequestedState = entries[1];
                    app.Instances = entries[2];
                    app.Memory = entries[3];
                    app.Disk = entries[4];
                    app.Alerts = entries[5] == "<none>" || entries[5].StartsWith("http") ? "" : entries[5];
                    app.Urls = entries[5] == "<none>" || entries[5].StartsWith("http") ? entries[5] : entries[6];
                    apps.Add(app);
                }
            }
            return apps;
        }

        private static List<XSARoute> GetRoutes()
        {
            string command = "xs routes";
            string cmdResult = RunCommand(command);

            List<XSARoute> routes = new List<XSARoute>();
            List<string> cmdResults = cmdResult.Split(Environment.NewLine).ToList();
            List<string> filteredResults = cmdResults.Skip(3).ToList();
            foreach (var result in filteredResults)
            {
                if (result != string.Empty)
                {
                    XSARoute route = new XSARoute();
                    string[] entries = result.Split(" ");
                    route.Name = entries[0];
                    route.Domain = entries[1];
                    route.Port = entries[2];
                    route.Path = entries[3];
                    route.Type = entries[4];
                    route.Apps = entries[5];
                    routes.Add(route);
                }
            }
            return routes;
        }

        private static Target GetTarget()
        {
            string command = "xs target";
            string cmdResult = RunCommand(command);

            Target target = new Target();
            string[] results = cmdResult.Split(Environment.NewLine);
            foreach (var result in results)
            {
                if (result.StartsWith("API endpoint:"))
                {
                    target.APIEndpoint = result.Replace("API endpoint:", "").Trim();
                }
                else if (result.StartsWith("User:"))
                {
                    target.User = result.Replace("User:", "").Trim();
                }
                else if (result.StartsWith("Org:"))
                {
                    target.Org = result.Replace("Org:", "").Trim();
                }
                else if (result.StartsWith("Space:"))
                {
                    target.Space = result.Replace("Space:", "").Trim();
                }
            }
            return target;
        }


        private static List<XSAService> GetServices()
        {
            string command = "xs services";
            string cmdResult = RunCommand(command);

            List<XSAService> services = new List<XSAService>();
            List<string> cmdResults = cmdResult.Split(Environment.NewLine).ToList();
            List<string> filteredResults = cmdResults.Skip(6).ToList();
            foreach (var result in filteredResults)
            {
                if (result != string.Empty)
                {
                    XSAService service = new XSAService();
                    string[] entries = result.Split(" ");
                    service.Name = entries[0];
                    service.Plan = entries[1];
                    service.LastOperation = entries[2];
                    service.BoundApps = entries[3];
                    service.Service = entries.Length >= 4 ? entries[4] : "";
                    services.Add(service);
                }
            }
            return services;
        }

        private static List<XSAUser> GetUsers()
        {
            string command = "xs users";
            string cmdResult = RunCommand(command);

            List<XSAUser> users = new List<XSAUser>();
            List<string> cmdResults = cmdResult.Split(Environment.NewLine).ToList();
            List<string> filteredResults = cmdResults.Skip(4).ToList();
            foreach (var result in filteredResults)
            {
                if (result != string.Empty)
                {
                    XSAUser user = new XSAUser();
                    string[] entries = result.Split(" ");
                    user.User = entries[0];
                    user.Origin = entries.Length >= 2 ? entries[1] : "";
                    user.Deactivated = entries.Length >= 3 ? entries[2] : "";
                    users.Add(user);
                }
            }
            return users;
        }

        private static string AbortMtaOp(string mtaOpId, string opType)
        {
            Thread.Sleep(500);
            string command = $"xs {opType.ToLower()} -i {mtaOpId} -a abort";
            string cmdResult = RunCommand(command);
            return cmdResult;
        }

        private static string DeleteService(string serviceName)
        {
            Thread.Sleep(500);
            string command = $"xs delete-service {serviceName} -f --purge";
            string cmdResult = RunCommand(command);
            return cmdResult;
        }

        private static string RunCommand(string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");
            bool isWindows = OperatingSystem.IsWindows();
            if (isWindows)
            {
                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/C \"{escapedArgs}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };
                process.Start();
                string result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return result;
            }
            else
            {
                //all others.
                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"{escapedArgs}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };
                process.Start();
                string result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return result;
            }
        }

        private static void PrintInstructions()
        {
            Console.WriteLine("xsext");
            Console.WriteLine("");
            Console.WriteLine("commands:");
            Console.WriteLine("------------------------------------");
            Console.WriteLine("delete-workspace-services | dws | optional param: whitelist=[matchStr1,matchStr2...]");
            Console.WriteLine("delete-all-workspace-services | daws | optional param: whitelist=[matchStr1,matchStr2...]");
            Console.WriteLine("delete-mta-ops-errors | dmoe");
            Console.WriteLine("delete-all-mta-ops-errors | damoe");
            Console.WriteLine("------------------------------------");
            Console.WriteLine("");

        }
    }

    public static class OperatingSystem
    {
        public static bool IsWindows() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool IsMacOS() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static bool IsLinux() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }

}
