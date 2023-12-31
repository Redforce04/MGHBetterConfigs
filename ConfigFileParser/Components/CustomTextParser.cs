﻿// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         ConfigFileParser
//    Project:          ConfigFileParser
//    FileName:         CustomTextParser.cs
//    Author:           Redforce04#4091
//    Revision Date:    07/26/2023 2:18 AM
//    Created Date:     07/26/2023 2:18 AM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using ConfigFileParser.Configs;
using ThreadState = System.Threading.ThreadState;

namespace ConfigFileParser.Components;

public class CustomTextParser
{
    public static CustomTextParser Singleton;
    private static string Billboard = @"<Accent>  __  __  _____ _    _   ____       _   _             _____             __ _           " + "\n" + @" |  \/  |/ ____| |  | | |  _ \     | | | |           / ____|           / _(_)           " + "\n" + @" | \  / | |  __| |__| | | |_) | ___| |_| |_ ___ _ __| |     ___  _ __ | |_ _  __ _ ___  " + "\n" + @" | |\/| | | |_ |  __  | |  _ < / _ | __| __/ _ | '__| |    / _ \| '_ \|  _| |/ _` / __| " + "\n" + @" | |  | | |__| | |  | | | |_) |  __| |_| ||  __| |  | |___| (_) | | | | | | | (_| \__ \ " + "\n" + @" |_|  |_|\_____|_|  |_| |____/ \___|\__|\__\___|_|   \_____\___/|_| |_|_| |_|\__, |___/ " + "\n" + @"                                                                              __/ |     " + "\n" + @"                                                                             |___/      ";
    private static string Banner = $"<SecondaryAccent>MGH BetterConfigs {VersionInfo.CommitVersion} - by Redforce04{(Config.Singleton.Debug ? $" <DarkGreen>[{VersionInfo.CommitBranch} - {VersionInfo.CommitHash}]" : "")}"; 

    public CustomTextParser()
    {
        Singleton = this;
    }

    public void PrintLine(string line)
    {
        Console.Clear();
        _printHeader();

        Print(line);
    }
    /// <summary>
    /// Deprecated.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="countdownDurationInSeconds"></param>
    /// <param name="readInput"></param>
    /// <returns></returns>
    private string? PrintCoundown(string text, float countdownDurationInSeconds = 5, bool readInput = true)
    {
        Thread _countdownThread = new Thread(() => _printCountdownInternally(text, countdownDurationInSeconds));
        _countdownThread.Name = "Countdown Text Thread";
        _countdownThread.Start();
        if (!readInput)
        {
            return null;
        }
        try
        {
            /*string query = Reader.ReadLine((int)countdownDurationInSeconds*1000);
            _countdownThread.Interrupt();
            return query;*/
        }
        catch (TimeoutException)
        {
            if (_countdownThread.ThreadState is ThreadState.Running or ThreadState.Background)
            {
                _countdownThread.Interrupt();
            }
        }
        // renames '{remainingDuration}' to time left, and '{totalDuration}' to the totalDuration.    
        // runs on an independent thread    
        return null;
    }

    private void _printCountdownInternally(string text, float countdownDuration)
    {
        try
        {

            if (!text.Contains("{remainingDuration"))
            {
                Print(text.Replace("{totalDuration}", ((int)countdownDuration).ToString()));
                Thread.Sleep((int)countdownDuration * 1000);
                return;
            }
            int totalDurationInMilliseconds = (int)countdownDuration * 1000;
            int remainingDuration = totalDurationInMilliseconds;
            Stopwatch stopwatch = new Stopwatch();
            while (remainingDuration > 0)
            {
                stopwatch.Start();
                Console.Clear();
                int rem = remainingDuration / 1000;
                int tot = (int)countdownDuration;

                string txt = (text
                    .Replace("{remainingDuration}", rem.ToString())
                    .Replace("{totalDuration}", tot.ToString()));
                Print(txt);
                int sleepDuration = (remainingDuration > 1000) ? 1000 : remainingDuration;
                stopwatch.Stop();
                Thread.Sleep(sleepDuration - (int)stopwatch.ElapsedMilliseconds);
                remainingDuration -= sleepDuration;
            }
        }
        catch (ThreadInterruptedException)
        {
            
        }
    }
    private void _printHeader()
    {
        if (!Config.Singleton.Silent)
        {
            Print(Billboard);
            int billboardLength = Billboard.Split('\n')[0].Length - _getCharacterWidth(Billboard.Split("\n")[0]);
            int bannerLength = Banner.Length + (_getCharacterWidth(Banner));
            int padLength =  (bannerLength + billboardLength)/2;// / 2;
            // padLength = billboardLength ;
           Print(Banner.PadLeft(padLength)+ "\n");
        }
        
        
    }
    //88 
//banner - 88
//<SecondaryAccent><DarkGreen> // 29
// 
//MGH BetterConfigs v1.0.2-beta - by Redforce04 [master - e6a8e098] //68

    public void PrintCustomInput(TextInfo info)
    {
        // 88
        // 88 - text.count /2

        Console.Clear();
        _printHeader();

        Print($"<Primary>Current Config: <Accent>{info.ConfigName} <Primary>({info.ConfigType}) - Config [<Accent>{info.CurrentConfigNum} <Primary>/ {info.TotalConfigNum}]");
        Print($"<Primary>{info.Description} (default: <Accent>{info.DefaultValue}<Primary>)");

        foreach (string instructionLine in info.Instruction.Split('\n'))
        {
            if(instructionLine != "")
                Print($"<Secondary>-- {instructionLine} --");
        }
        foreach (string errorLine in info.ErrorString.Split('\n'))
        {
            Print($"{errorLine}");
        }
        info.ErrorString = "";
        foreach(string str in info.CustomStrings)
        {
            Print(str);
        }

        info.CustomStrings = new List<string>();
    }

    public void PrintConfigSummary(TextInfo info)
    {
        Console.Clear();
        _printHeader();

        Print($"{info.Description}");
        /*foreach (string instructionLine in info.Instruction.Split('\n'))
        {
            Print($"<Secondary>-- {instructionLine} --");
        }*/

        info.ErrorString = "";
        foreach(string str in info.CustomStrings)
        {
            Print(str);
        }
    }

    private int _getCharacterWidth(string txt)
    {
        string colors = "|";
        foreach (ConsoleColor en in Enum.GetValuesAsUnderlyingType<ConsoleColor>())
        {
            colors += $"|{en.ToString()}";
        }

        colors = colors.Replace("||", "");
        colors += "|Primary|Accent|SecondaryAccent|Secondary|Warn|Error";
        var regex = new Regex(@"<(" + colors + @".*?)>");
        var matches = regex.Matches(txt);
        int length = 0;
        foreach (Match match in matches)
        {
            length += match.Length;
        }
        Console.WriteLine();

        return length;
    } 
    internal void Print(string text)
    {
        string newText = text;
        List<KeyValuePair<int, ConsoleColor>> Lines = new List<KeyValuePair<int, ConsoleColor>>();
        Lines.Add(new KeyValuePair<int, ConsoleColor>(0, ConsoleColor.White));
        string colors = "|";
        foreach (ConsoleColor en in Enum.GetValuesAsUnderlyingType<ConsoleColor>())
        {
            colors += $"|{en.ToString()}";
        }

        colors = colors.Replace("||", "");
        colors += "|Primary|Accent|SecondaryAccent|Secondary|Warn|Error";
        var regex = new Regex(@"<(" + colors + @".*?)>");
        var matches = regex.Matches(text);
        int displacement = 0;
        if (matches.Count < 1)
        {
            Console.WriteLine(text);
            return;
        }
        foreach (Match match in matches)
        {
            if (match.Groups.Count < 2)
            {
                //if(Config.Singleton.Debug) Console.WriteLine($"No group found.");
                continue;
            }
            string val = newText.Substring(0, match.Index - displacement);

            ConsoleColor color = ActiveColorScheme["Primary"];
            try
            {
                color = Enum.Parse<ConsoleColor>(match.Groups[1].Value, true);
            }
            catch (ArgumentException)
            {
                if (ConsoleColorScheme.ContainsKey(match.Groups[1].Value))
                {
                    color = ConsoleColorScheme[match.Groups[1].Value];
                }
                else
                {
                    continue;
                }
            }
            catch (Exception e)
            {
                if(Config.Singleton.Debug) Console.WriteLine($"{e}");
                continue;
            }

            newText = newText.Remove(match.Index-displacement, match.Length);
            //newText = text.Substring(match.Index + match.Length, (newText.Length - (match.Index + match.Length)));
            Lines.Add(new KeyValuePair<int, ConsoleColor>(match.Index - displacement, color));
            displacement += match.Length;
            //Console.WriteLine($"{color}");
        }

        for (int i = 0; i < Lines.Count; i++)
        {
            KeyValuePair<int, ConsoleColor> pair = Lines[i];
            int length = 0;
            if (i + 1 < Lines.Count)
            {
                length = Lines[i + 1].Key - pair.Key;
            }
            else
            {
                length = newText.Length - pair.Key;
            }

            string line = newText.Substring(pair.Key, length);
            Console.ForegroundColor = pair.Value;
            Console.Write(line);
        }
        Console.WriteLine();
        Console.ForegroundColor = ActiveColorScheme["Primary"];;
    }

    public static Dictionary<string, ConsoleColor> ActiveColorScheme => Config.ExportedColorScheme switch
    {
        ColorSchemes.Windows => ConsoleColorScheme,
        ColorSchemes.Linux => LinuxColorScheme,
        ColorSchemes.Pterodactyl => PterodactylColorScheme,
        _ => ConsoleColorScheme
    };

    public static Dictionary<string, ConsoleColor> ConsoleColorScheme = new Dictionary<string, ConsoleColor>()
    {
        { "Primary", ConsoleColor.Gray },
        { "Secondary", ConsoleColor.DarkGray },
        { "Accent", ConsoleColor.DarkBlue },
        { "SecondaryAccent", ConsoleColor.DarkMagenta },
        { "Warn", ConsoleColor.Red },
        { "Error", ConsoleColor.Red },
    };
    public static Dictionary<string, ConsoleColor> PterodactylColorScheme = new Dictionary<string, ConsoleColor>()
    {
        { "Primary", ConsoleColor.White },
        { "Secondary", ConsoleColor.Gray },
        { "SecondaryAccent", ConsoleColor.DarkRed },
        { "Accent", ConsoleColor.Cyan },
        { "Warn", ConsoleColor.DarkRed },
        { "Error", ConsoleColor.Red },
    };
    public static Dictionary<string, ConsoleColor> LinuxColorScheme = new Dictionary<string, ConsoleColor>()
    {
        { "Primary", ConsoleColor.White },
        { "Secondary", ConsoleColor.Gray },
        { "SecondaryAccent", ConsoleColor.DarkRed },
        { "Accent", ConsoleColor.Cyan },
        { "Warn", ConsoleColor.DarkRed },
        { "Error", ConsoleColor.Red },
    };
}

public class TextInfo
{
    public string ConfigName = "";
    public string ConfigFullName = "";
    public string ConfigType = "";
    public int CurrentConfigNum = 0;
    public int TotalConfigNum = 0;
    public int CurrentTry = 0;
    public string Instruction = "";
    public string Description = "";
    public string DefaultValue = "";
    public string ErrorString = "";
    public List<string> CustomStrings = new List<string>();
}

/*
 MGH BetterConfigs
 Hunter Movement Speed Multiplier (Integer) - Config [3 / 19]
 -- Type value or leave blank for default value, then press enter to submit. --
 What should the speed multiplier be for ghosts? (default: 1.0)  
 */