using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebApplication1 {
    public class Startup {
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.Use(async (context, next) => {
                // if contentType is not application/xml - do nothing
                if (context.Request.Method == "POST" && context.Request.ContentType == "application/xml") {
                    // context.Request.Body is HttpRequestStream
                    // it is not rewindable, it means, you can not change Position
                    // so, it can not be read twice
                    var originalStream = context.Request.Body;

                    // but we NEED to read it twice, so, we create MemoryStream, which IS rewindable
                    using (var buffer = new MemoryStream()) {
                        // and copy content from context.Request.Body to this MemoryStream
                        await originalStream.CopyToAsync(buffer);
                        // rewind stream
                        buffer.Position = 0;

                        // do whatever you need with this stream
                        using (var reader = new StreamReader(buffer, leaveOpen: true)) {
                            var str = await reader.ReadToEndAsync();
                            context.Items.Add("data", str);
                        }

                        // rewind it back
                        buffer.Position = 0;

                        // replace original stream with MemoryStream, containing body content
                        context.Request.Body = buffer;
                    }
                }

                await next.Invoke();
            });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}