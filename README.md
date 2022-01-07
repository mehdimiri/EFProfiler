# EFProfiler
Entity Framework Profiler (EFProfiler) is a tool for finding slow queries
<h2> Install via NuGet</h2>
To install EFProfiler, run the following command in Package Manager Console
<pre lang="code">
<code>
    pm> Install-Package EFProfiler
</code>
</pre>
<p>You can also view the <a href="https://www.nuget.org/packages/EFProfiler/" rel="nofollow">package page</a> on NuGet.</p>
<h2>How to use ?</h2>
Add required services Startup class as below :

<pre lang="code">
<code>
     services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("DataContext"));
                options.AddInterceptors(new DatabaseQueryLogger(10, $"wwwroot\\LogFile\\"));
            });
</code>
</pre>

To display the logs :
<pre lang="code">
<code>
     app.EFProfilerUI(new EFProfilerUIOptions { HeadContent= "EFProfiler - Mehdi Miri"});
</code>
</pre>
Configuring Dashboard authorization :
<pre lang="code">
<code>
    app.UseAuthentication();
    app.UseAuthorization();

    app.EFProfilerUI(new EFProfilerUIOptions { Authorization = new EFProfleAuthorization { Roles = "admin" ,Users="Mehdi" } });

</code>
</pre>

Then enter the following address in the browser :
<pre lang="code">
<code>
    https://{sitename}/efprofiler/index.html
</code>
</pre>
![image](https://drive.google.com/uc?export=view&id=18NC5LrTBBhaZOhaLoyoV12yY0G58gnrr)
