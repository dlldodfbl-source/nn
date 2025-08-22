using Microsoft.AspNetCore.Mvc;
using static PROtofile.Date.DTOS.ChatDto;
using System.Text.Json;
using System.Net.Http.Json;


namespace PROtofile.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IHttpClientFactory _http;
    private readonly IConfiguration _cfg;

    public ChatController(IHttpClientFactory http, IConfiguration cfg)
    {
        _http = http; _cfg = cfg;
    }

    // كلمات بسيطة تحدد إن السؤال برمجي (عربي/إنجليزي)
    private static readonly string[] DevHints = [
        "code","bug","fix","api","class","function","algorithm","sql","entity","jwt","swagger","regex",
        "javascript","typescript","c#","asp.net","ef core","react","vue","angular","html","css","sass",
        "git","github","deploy","server","linux","windows","devops","async","await",
        "باك اند","فرونت","كود","برمجة","خوارزمية","دالة","كلاس","قاعدة بيانات"
    ];
    private static bool IsProgrammingQuestion(string text)
        => DevHints.Any(k => (text ?? "").ToLowerInvariant().Contains(k));

    [HttpPost]
    public async Task<ActionResult<ChatResponseDto>> Send([FromBody] ChatRequestDto req)
    {
        if (req?.Messages is null || req.Messages.Count == 0)
            return BadRequest("messages required");

        var lastUser = req.Messages.LastOrDefault(m => m.role == "user")?.content ?? "";
        if (!IsProgrammingQuestion(lastUser))
        {
            return Ok(new ChatResponseDto
            {
                Reply = "أنا بوت للبرمجة فقط 🧑‍💻. اسألني عن C#, ASP.NET, JavaScript, SQL… وهردّ بإجابات عملية وكود."
            });
        }

        // معلومات البوت من appsettings.json
        var persona = _cfg["Bot:Persona"] ?? "بوت برمجة.";
        var name = _cfg["Bot:Name"] ?? "Nexus";
        var factsArr = _cfg.GetSection("Bot:PersonalFacts").Get<string[]>() ?? Array.Empty<string>();
        var facts = factsArr.Length > 0
            ? "- " + string.Join("\n- ", factsArr)
            : "- لا توجد معلومات شخصية إضافية.";

        var systemPrompt = $@"
أنت {name}، {persona}
القواعد:
- أجب فقط على أسئلة البرمجة/السوفتوير. أي سؤال خارج ذلك: قل ""أنا بوت للبرمجة فقط.""
- أعطِ حلول عملية مع كود صحيح مختصر وتعليل بسيط.
- استخدم العربية الفصحى، ومصطلحات تقنية إنجليزية عند الحاجة.
- معلومات عن صاحب الموقع:
{facts}
";

        // شكل OpenAI-compatible
        var messages = new List<object> { new { role = "system", content = systemPrompt } };
        messages.AddRange(req.Messages.Select(m => new { m.role, m.content }));

        var body = new
        {
            model = _cfg["DeepSeek:Model"] ?? "deepseek-chat",
            messages,
            temperature = double.TryParse(_cfg["DeepSeek:Temperature"], out var t) ? t : 0.2
        };

        var http = _http.CreateClient("DeepSeek");
        using var res = await http.PostAsJsonAsync("chat/completions", body);
        if (!res.IsSuccessStatusCode)
        {
            var raw = await res.Content.ReadAsStringAsync();
            return StatusCode((int)res.StatusCode, new { error = raw });
        }

        var json = await res.Content.ReadFromJsonAsync<JsonElement>();
        var content = json.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

        return Ok(new ChatResponseDto { Reply = content ?? "" });
    }
}