using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace BooksApi.Middleware;
public class JsonToHtmlMiddleware
{
    private readonly RequestDelegate _next;

    public JsonToHtmlMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Capture the original response body stream
        var originalBodyStream = context.Response.Body;

        // Use a memory stream to temporarily store the response
        using (var newBodyStream = new MemoryStream())
        {
            context.Response.Body = newBodyStream;

            // Continue processing the request
            await _next(context);

            // Check if the response content type is JSON
            if (context.Response.ContentType != null && context.Response.ContentType.Contains("application/json"))
            {
                // Read the JSON from the temporary body stream
                newBodyStream.Seek(0, SeekOrigin.Begin);
                var jsonResponse = await new StreamReader(newBodyStream).ReadToEndAsync();

                // Convert the JSON to HTML
                var htmlResponse = ConvertJsonToHtml(jsonResponse);

                // Replace the response content with the HTML
                context.Response.ContentType = "text/html";
                context.Response.Body = originalBodyStream;
                await context.Response.WriteAsync(htmlResponse);
            }
            else
            {
                // If not JSON, just copy the original stream back
                newBodyStream.Seek(0, SeekOrigin.Begin);
                await newBodyStream.CopyToAsync(originalBodyStream);
            }
        }
    }

   private string ConvertJsonToHtml(string jsonResponse)
    {
        var jsonDocument = JsonDocument.Parse(jsonResponse);
        var htmlBuilder = new System.Text.StringBuilder();


        if (jsonDocument.RootElement.ValueKind == JsonValueKind.Object)
        {
            htmlBuilder.Append("<table border=\"1\">");

            foreach (var element in jsonDocument.RootElement.EnumerateObject())
            {
                htmlBuilder.Append("<tr>");
                htmlBuilder.AppendFormat("<td><strong>{0}</strong></td>", element.Name);
                htmlBuilder.AppendFormat("<td>{0}</td>", element.Value);
                htmlBuilder.Append("</tr>");
            }

            htmlBuilder.Append("</table>");
        }
        else if (jsonDocument.RootElement.ValueKind == JsonValueKind.Array)
        {
        var arrayItems = jsonDocument.RootElement.EnumerateArray().ToList();

        if (arrayItems.Count > 0 && arrayItems[0].ValueKind == JsonValueKind.Object)
        {
            htmlBuilder.Append("<table class=\"table table-striped\">"); // PicoCSS table with striped rows
            htmlBuilder.Append("<thead><tr>");

            // Get headers from the first object's keys
            foreach (var header in arrayItems[0].EnumerateObject())
            {
                htmlBuilder.AppendFormat("<th>{0}</th>", header.Name);
            }

            htmlBuilder.Append("</tr></thead><tbody>");

            // Add rows
            foreach (var item in arrayItems)
            {
                htmlBuilder.Append("<tr>");

                foreach (var value in item.EnumerateObject())
                {
                    htmlBuilder.AppendFormat("<td>{0}</td>", value.Value);
                }

                htmlBuilder.Append("</tr>");
            }

            htmlBuilder.Append("</tbody></table>");
        }
        else
        {
            htmlBuilder.Append("<ul>");

            foreach (var item in arrayItems)
            {
                htmlBuilder.Append("<li>");
                htmlBuilder.Append(item.ToString());
                htmlBuilder.Append("</li>");
            }

            htmlBuilder.Append("</ul>");
        }
    }


        return htmlBuilder.ToString();
    }

}
