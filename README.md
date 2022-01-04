# EFProfiler
Entity Framework Profiler (EFProfiler) is a tool for finding slow queries

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

To display the logs as follows :
<pre lang="code">
<code>
     app.EFProfilerUI(new EFProfilerUIOptions { HeadContent= "EFProfiler - Mehdi Miri"});
</code>
</pre>
