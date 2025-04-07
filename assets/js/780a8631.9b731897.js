"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[6961],{28453:(e,n,s)=>{s.d(n,{R:()=>a,x:()=>r});var i=s(96540);const t={},o=i.createContext(t);function a(e){const n=i.useContext(o);return i.useMemo((function(){return"function"==typeof e?e(n):{...n,...e}}),[n,e])}function r(e){let n;return n=e.disableParentContext?"function"==typeof e.components?e.components(t):e.components||t:a(e.components),i.createElement(o.Provider,{value:n},e.children)}},90508:(e,n,s)=>{s.r(n),s.d(n,{assets:()=>l,contentTitle:()=>a,default:()=>h,frontMatter:()=>o,metadata:()=>r,toc:()=>d});var i=s(74848),t=s(28453);const o={sidebar_position:1,description:"KafkaFlow is a .NET framework to create Kafka-based applications, simple to use and extend.",sidebar_label:"Introduction",slug:"/"},a="Introduction to KafkaFlow",r={id:"introduction",title:"Introduction to KafkaFlow",description:"KafkaFlow is a .NET framework to create Kafka-based applications, simple to use and extend.",source:"@site/versioned_docs/version-2.x/introduction.md",sourceDirName:".",slug:"/",permalink:"/kafkaflow/docs/2.x/",draft:!1,unlisted:!1,editUrl:"https://github.com/farfetch/kafkaflow/tree/master/website/versioned_docs/version-2.x/introduction.md",tags:[],version:"2.x",sidebarPosition:1,frontMatter:{sidebar_position:1,description:"KafkaFlow is a .NET framework to create Kafka-based applications, simple to use and extend.",sidebar_label:"Introduction",slug:"/"},sidebar:"tutorialSidebar",next:{title:"Getting Started",permalink:"/kafkaflow/docs/2.x/category/getting-started"}},l={},d=[{value:"Features",id:"features",level:2},{value:"Join the community",id:"join-the-community",level:2},{value:"Something missing here?",id:"something-missing",level:2},{value:"License",id:"license",level:2}];function c(e){const n={a:"a",h1:"h1",h2:"h2",li:"li",p:"p",strong:"strong",ul:"ul",...(0,t.R)(),...e.components};return(0,i.jsxs)(i.Fragment,{children:[(0,i.jsx)(n.h1,{id:"introduction-to-kafkaflow",children:"Introduction to KafkaFlow"}),"\n",(0,i.jsxs)(n.p,{children:["\u26a1\ufe0f KafkaFlow was designed to build .NET applications on top of Apache Kafka ",(0,i.jsx)(n.strong,{children:"in a simple and maintainable way"}),"."]}),"\n",(0,i.jsxs)(n.p,{children:["\ud83c\udfd7 Built on top of ",(0,i.jsx)(n.a,{href:"https://github.com/confluentinc/confluent-kafka-dotnet",children:"Confluent Kafka Client"}),"."]}),"\n",(0,i.jsx)(n.p,{children:"\ud83d\udd0c Extensible by design."}),"\n",(0,i.jsxs)(n.p,{children:["Get started building by installing ",(0,i.jsx)(n.a,{href:"/kafkaflow/docs/2.x/getting-started/installation",children:"KafkaFlow"})," or following our ",(0,i.jsx)(n.a,{href:"/kafkaflow/docs/2.x/getting-started/create-your-first-application",children:"Quickstart"}),"."]}),"\n",(0,i.jsx)(n.h2,{id:"features",children:"Features"}),"\n",(0,i.jsx)(n.p,{children:"Our goal is to empower you to build event-driven applications on top of Apache Kafka.\nTo do that, KafkaFlow gives you access to features like:"}),"\n",(0,i.jsxs)(n.ul,{children:["\n",(0,i.jsx)(n.li,{children:"Multi-threaded consumer with message order guarantee."}),"\n",(0,i.jsxs)(n.li,{children:[(0,i.jsx)(n.a,{href:"guides/middlewares/",children:"Middlewares"})," support for producing and consuming messages."]}),"\n",(0,i.jsx)(n.li,{children:"Support topics with different message types."}),"\n",(0,i.jsx)(n.li,{children:"Consumers with many topics."}),"\n",(0,i.jsxs)(n.li,{children:[(0,i.jsx)(n.a,{href:"/kafkaflow/docs/2.x/guides/middlewares/serializer-middleware",children:"Serializer middleware"})," with ",(0,i.jsx)(n.strong,{children:"ApacheAvro"}),", ",(0,i.jsx)(n.strong,{children:"ProtoBuf"})," and ",(0,i.jsx)(n.strong,{children:"Json"})," algorithms."]}),"\n",(0,i.jsxs)(n.li,{children:[(0,i.jsx)(n.a,{href:"/kafkaflow/docs/2.x/guides/middlewares/serializer-middleware#adding-schema-registry-support",children:"Schema Registry"})," support."]}),"\n",(0,i.jsxs)(n.li,{children:[(0,i.jsx)(n.a,{href:"/kafkaflow/docs/2.x/guides/compression",children:"Compression"})," using native Confluent Kafka client compression or compressor middleware."]}),"\n",(0,i.jsxs)(n.li,{children:[(0,i.jsx)(n.a,{href:"/kafkaflow/docs/2.x/guides/global-events",children:"Global Events Subcription"})," for message production and consumption."]}),"\n",(0,i.jsxs)(n.li,{children:[(0,i.jsx)(n.a,{href:"/kafkaflow/docs/2.x/guides/open-telemetry",children:"Open Telemetry Instrumentation"})," for traces and baggage signals."]}),"\n",(0,i.jsx)(n.li,{children:"Graceful shutdown (wait to finish processing to shutdown)."}),"\n",(0,i.jsx)(n.li,{children:"Store offset when processing ends, avoiding message loss."}),"\n",(0,i.jsx)(n.li,{children:"Supports .NET Core and .NET Framework."}),"\n",(0,i.jsxs)(n.li,{children:["Can be used with any dependency injection framework (see ",(0,i.jsx)(n.a,{href:"/kafkaflow/docs/2.x/guides/dependency-injection",children:"here"}),")."]}),"\n",(0,i.jsx)(n.li,{children:"Fluent configuration."}),"\n",(0,i.jsxs)(n.li,{children:[(0,i.jsx)(n.a,{href:"/kafkaflow/docs/2.x/guides/admin/web-api",children:"Admin Web API"})," that allows pause, resume and restart consumers, change workers count, and rewind offsets, ",(0,i.jsx)(n.strong,{children:"all at runtime"}),"."]}),"\n",(0,i.jsxs)(n.li,{children:[(0,i.jsx)(n.a,{href:"/kafkaflow/docs/2.x/guides/admin/dashboard",children:"Dashboard UI"})," that allows to visualize of relevant information about all consumers and managing them."]}),"\n"]}),"\n",(0,i.jsx)(n.h2,{id:"join-the-community",children:"Join the community"}),"\n",(0,i.jsxs)(n.ul,{children:["\n",(0,i.jsxs)(n.li,{children:[(0,i.jsx)(n.a,{href:"https://github.com/farfetch/kafkaflow",children:"GitHub"}),": For new feature requests, bug reporting or contributing with your own pull request."]}),"\n",(0,i.jsxs)(n.li,{children:[(0,i.jsx)(n.a,{href:"https://join.slack.com/t/kafkaflow/shared_invite/zt-puihrtcl-NnnylPZloAiVlQfsw~RD6Q",children:"Slack"}),": The place to be if you want to ask questions or share ideas."]}),"\n",(0,i.jsxs)(n.li,{children:[(0,i.jsx)(n.a,{href:"https://stackoverflow.com/questions/tagged/kafkaflow",children:"Stack Overflow"}),": The best place to start if you have a question."]}),"\n"]}),"\n",(0,i.jsx)(n.h2,{id:"something-missing",children:"Something missing here?"}),"\n",(0,i.jsxs)(n.p,{children:["If you have suggestions to improve the documentation, please send us a new ",(0,i.jsx)(n.a,{href:"https://github.com/farfetch/kafkaflow/issues",children:"issue"}),"."]}),"\n",(0,i.jsx)(n.h2,{id:"license",children:"License"}),"\n",(0,i.jsxs)(n.p,{children:["KafkaFlow is a free and open source project, released under the permissible ",(0,i.jsx)(n.a,{href:"https://github.com/Farfetch/kafkaflow/blob/master/LICENSE",children:"MIT license"}),"."]})]})}function h(e={}){const{wrapper:n}={...(0,t.R)(),...e.components};return n?(0,i.jsx)(n,{...e,children:(0,i.jsx)(c,{...e})}):c(e)}}}]);