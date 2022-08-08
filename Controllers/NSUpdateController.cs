using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Diagnostics;



namespace nsupdateapi.Controllers;
[ApiController]
[Route("[controller]")]
public class NSUpdateController : ControllerBase
{
    private readonly ILogger<NSUpdateController> _logger;
    private CustomConfiguration _configuration;
    public NSUpdateController(ILogger<NSUpdateController> logger, CustomConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost()]
    public async Task<IActionResult> ExecuteNSUpdateScript(List<string> scriptLines)
    {
        var dnsServers = _configuration.DnsServers;
        bool updateAllDNS = _configuration.UpdateAllDnsServers;
        bool success;
        if (scriptLines.Count == 0)
        {
            return BadRequest("Must have at least one command");
        }

        if (updateAllDNS)
        {
            success = true;
            foreach (var d in dnsServers)
            {
                string command = string.Format("echo -e \"server {0}\n", d);

                foreach (var line in scriptLines)
                {
                    command+= line + "\n";
                }
                command+= "send\" | nsupdate -D";
                Console.WriteLine(command);
                if (await command.Bash(_logger) != 0) success = false;
            }
        }
        else {
            success = false;
            foreach (var d in dnsServers)
            {
                string command = string.Format("echo -e \"server {0}\n", d);

                foreach (var line in scriptLines)
                {
                    command+= line + "\n";
                }
                command+= "send\" | nsupdate -D";
                Console.WriteLine(command);
                if (await command.Bash(_logger) == 0)
                {
                    success = true;
                    break;
                }
            }
        }

        if (success)
        {
            return Created("", null);
        }
        else
        {
            return StatusCode(500, "Error encountered, check logs");
        }
    }
}