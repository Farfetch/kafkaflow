"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[5718],{28453:(e,s,r)=>{r.d(s,{R:()=>n,x:()=>t});var o=r(96540);const d={},i=o.createContext(d);function n(e){const s=o.useContext(i);return o.useMemo((function(){return"function"==typeof e?e(s):{...s,...e}}),[s,e])}function t(e){let s;return s=e.disableParentContext?"function"==typeof e.components?e.components(d):e.components||d:n(e.components),o.createElement(i.Provider,{value:s},e.children)}},35044:(e,s,r)=>{r.r(s),r.d(s,{assets:()=>a,contentTitle:()=>n,default:()=>m,frontMatter:()=>i,metadata:()=>t,toc:()=>c});var o=r(74848),d=r(28453);const i={sidebar_position:4},n="Compressor Middleware",t={id:"guides/middlewares/compressor-middleware",title:"Compressor Middleware",description:"In this section, we will learn how to build a custom Compressor Middleware.",source:"@site/docs/guides/middlewares/compressor-middleware.md",sourceDirName:"guides/middlewares",slug:"/guides/middlewares/compressor-middleware",permalink:"/kafkaflow/docs/guides/middlewares/compressor-middleware",draft:!1,unlisted:!1,editUrl:"https://github.com/farfetch/kafkaflow/tree/master/website/docs/guides/middlewares/compressor-middleware.md",tags:[],version:"current",sidebarPosition:4,frontMatter:{sidebar_position:4},sidebar:"tutorialSidebar",previous:{title:"Serializer Middleware",permalink:"/kafkaflow/docs/guides/middlewares/serializer-middleware"},next:{title:"Batch Consume Middleware",permalink:"/kafkaflow/docs/guides/middlewares/batch-consume-middleware"}},a={},c=[{value:"Add a Compressor Middleware",id:"add-a-compressor-middleware",level:2}];function l(e){const s={a:"a",admonition:"admonition",code:"code",h1:"h1",h2:"h2",p:"p",pre:"pre",...(0,d.R)(),...e.components};return(0,o.jsxs)(o.Fragment,{children:[(0,o.jsx)(s.h1,{id:"compressor-middleware",children:"Compressor Middleware"}),"\n",(0,o.jsx)(s.p,{children:"In this section, we will learn how to build a custom Compressor Middleware."}),"\n",(0,o.jsx)(s.admonition,{type:"warning",children:(0,o.jsxs)(s.p,{children:["It's recommended to use the producer's native compression instead of the compressor middleware. See ",(0,o.jsx)(s.a,{href:"../compression",children:"here"})," how to use it."]})}),"\n",(0,o.jsx)(s.p,{children:"If you want to build your own way of compress and decompress messages, you can find in this section the needed instructions."}),"\n",(0,o.jsx)(s.h2,{id:"add-a-compressor-middleware",children:"Add a Compressor Middleware"}),"\n",(0,o.jsxs)(s.p,{children:["Add the ",(0,o.jsx)(s.code,{children:"AddCompressor"}),"/",(0,o.jsx)(s.code,{children:"AddDecompressor"})," extension method to your producer/consumer middlewares to use it."]}),"\n",(0,o.jsxs)(s.p,{children:["The method receives a class that implements the ",(0,o.jsx)(s.code,{children:"ICompressor"}),"/",(0,o.jsx)(s.code,{children:"IDecompressor"})," interface as a generic argument. This class will be used in the compress/decompress process."]}),"\n",(0,o.jsx)(s.p,{children:"A class instance can be provided as an argument through a factory method too."}),"\n",(0,o.jsxs)(s.p,{children:["Install the ",(0,o.jsx)(s.a,{href:"https://www.nuget.org/packages/KafkaFlow.Compressor.Gzip/",children:"KafkaFlow.Compressor.Gzip"})," package to use the ",(0,o.jsx)(s.code,{children:"GzipMessageCompressor"}),"/",(0,o.jsx)(s.code,{children:"GzipMessageDecompressor"})," that uses the GZIP algorithm."]}),"\n",(0,o.jsx)(s.pre,{children:(0,o.jsx)(s.code,{className:"language-csharp",children:'public class Startup\n{\n    public void ConfigureServices(IServiceCollection services)\n    {\n        services.AddKafka(kafka => kafka\n            .AddCluster(cluster => cluster\n                .WithBrokers(new[] { "localhost:9092" })\n                .AddProducer<ProductEventsProducer>(producer => producer\n                    ...\n                    .AddMiddlewares(middlewares => middlewares\n                        ...\n                        .AddCompressor<GzipMessageCompressor>()\n                        // or\n                        .AddCompressor(resolver => new GzipMessageCompressor(...))\n                        ...\n                    )\n                )\n            )\n        );\n    }\n}\n'})})]})}function m(e={}){const{wrapper:s}={...(0,d.R)(),...e.components};return s?(0,o.jsx)(s,{...e,children:(0,o.jsx)(l,{...e})}):l(e)}}}]);