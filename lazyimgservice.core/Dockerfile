FROM microsoft/aspnetcore:2.0
RUN ln -s /lib/x86_64-linux-gnu/libdl.so.2 /lib/x86_64-linux-gnu/libdl.so  # These lines are needed for System.Drawing
RUN apt-get update                                                         # 
RUN apt install -y libgdiplus                                              #
RUN ln -s /usr/lib/libgdiplus.so /lib/x86_64-linux-gnu/libgdiplus.so       #
ARG source
WORKDIR /app # IMPORTANT!
EXPOSE 80
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "lazyimgservice.core.dll", "--urls", "http://*:5000"]
