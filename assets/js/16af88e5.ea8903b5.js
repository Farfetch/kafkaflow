"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[2313],{6825:(e,s,n)=>{n.r(s),n.d(s,{assets:()=>c,contentTitle:()=>a,default:()=>u,frontMatter:()=>o,metadata:()=>i,toc:()=>d});var r=n(74848),t=n(28453);const o={sidebar_position:9},a="Global Events",i={id:"guides/global-events",title:"Global Events",description:"In this section, we will delve into the concept of Global Events in KafkaFlow, which provides a mechanism to subscribe to various events that are triggered during the message production and consumption processes.",source:"@site/versioned_docs/version-2.x/guides/global-events.md",sourceDirName:"guides",slug:"/guides/global-events",permalink:"/kafkaflow/docs/2.x/guides/global-events",draft:!1,unlisted:!1,editUrl:"https://github.com/farfetch/kafkaflow/tree/master/website/versioned_docs/version-2.x/guides/global-events.md",tags:[],version:"2.x",sidebarPosition:9,frontMatter:{sidebar_position:9},sidebar:"tutorialSidebar",previous:{title:"Authentication",permalink:"/kafkaflow/docs/2.x/guides/authentication"},next:{title:"OpenTelemetry instrumentation",permalink:"/kafkaflow/docs/2.x/guides/open-telemetry"}},c={},d=[{value:"Message Produce Started Event",id:"message-produce-started-event",level:2},{value:"Message Produce Completed Event",id:"message-produce-completed-event",level:2},{value:"Message Produce Error Event",id:"message-produce-error-event",level:2},{value:"Message Consume Started Event",id:"message-consume-started-event",level:2},{value:"Message Consume Completed Event",id:"message-consume-completed-event",level:2},{value:"Message Consume Error Event",id:"message-consume-error-event",level:2}];function l(e){const s={a:"a",admonition:"admonition",code:"code",h1:"h1",h2:"h2",li:"li",p:"p",pre:"pre",ul:"ul",...(0,t.R)(),...e.components};return(0,r.jsxs)(r.Fragment,{children:[(0,r.jsx)(s.h1,{id:"global-events",children:"Global Events"}),"\n",(0,r.jsx)(s.p,{children:"In this section, we will delve into the concept of Global Events in KafkaFlow, which provides a mechanism to subscribe to various events that are triggered during the message production and consumption processes."}),"\n",(0,r.jsx)(s.p,{children:"KafkaFlow offers a range of Global Events that can be subscribed to. These events can be used to monitor and react to different stages of message handling. Below is a list of available events:"}),"\n",(0,r.jsxs)(s.ul,{children:["\n",(0,r.jsx)(s.li,{children:(0,r.jsx)(s.a,{href:"#message-produce-started-event",children:"Message Produce Started Event"})}),"\n",(0,r.jsx)(s.li,{children:(0,r.jsx)(s.a,{href:"#message-produce-completed-event",children:"Message Produce Completed Event"})}),"\n",(0,r.jsx)(s.li,{children:(0,r.jsx)(s.a,{href:"#message-produce-error-event",children:"Message Produce Error Event"})}),"\n",(0,r.jsx)(s.li,{children:(0,r.jsx)(s.a,{href:"#message-consume-started-event",children:"Message Consume Started Event"})}),"\n",(0,r.jsx)(s.li,{children:(0,r.jsx)(s.a,{href:"#message-consume-completed-event",children:"Message Consume Completed Event"})}),"\n",(0,r.jsx)(s.li,{children:(0,r.jsx)(s.a,{href:"#message-consume-error-event",children:"Message Consume Error Event"})}),"\n"]}),"\n",(0,r.jsx)(s.h2,{id:"message-produce-started-event",children:"Message Produce Started Event"}),"\n",(0,r.jsx)(s.p,{children:"The Message Produce Started Event is triggered when the message production process begins. It provides an opportunity to perform tasks or gather information before middlewares execution."}),"\n",(0,r.jsx)(s.pre,{children:(0,r.jsx)(s.code,{className:"language-csharp",children:"services.AddKafka(\n    kafka => kafka\n        .SubscribeGlobalEvents(observers =>\n        {\n            observers.MessageProduceStarted.Subscribe(eventContext =>\n            {\n                // Add your logic here\n            });\n        })\n"})}),"\n",(0,r.jsx)(s.h2,{id:"message-produce-completed-event",children:"Message Produce Completed Event"}),"\n",(0,r.jsx)(s.p,{children:"The Message Produce Completed Event is triggered when a message is successfully produced or when error messages occur during the production process. Subscribing to this event enables you to track the successful completion of message production."}),"\n",(0,r.jsx)(s.pre,{children:(0,r.jsx)(s.code,{className:"language-csharp",children:"services.AddKafka(\n    kafka => kafka\n        .SubscribeGlobalEvents(observers =>\n        {\n            observers.MessageProduceCompleted.Subscribe(eventContext =>\n            {\n                // Add your logic here\n            });\n        })\n"})}),"\n",(0,r.jsx)(s.h2,{id:"message-produce-error-event",children:"Message Produce Error Event"}),"\n",(0,r.jsx)(s.p,{children:"In case an error occurs during message production, the Message Produce Error Event is triggered. By subscribing to this event, you will be able to catch any exceptions that may occur while producing a message."}),"\n",(0,r.jsx)(s.pre,{children:(0,r.jsx)(s.code,{className:"language-csharp",children:"services.AddKafka(\n    kafka => kafka\n        .SubscribeGlobalEvents(observers =>\n        {\n            observers.MessageProduceError.Subscribe(eventContext =>\n            {\n                // Add your logic here\n            });\n        })\n"})}),"\n",(0,r.jsx)(s.h2,{id:"message-consume-started-event",children:"Message Consume Started Event"}),"\n",(0,r.jsx)(s.p,{children:"The Message Consume Started Event is raised at the beginning of the message consumption process. It offers an opportunity to execute specific tasks or set up resources before message processing begins."}),"\n",(0,r.jsx)(s.pre,{children:(0,r.jsx)(s.code,{className:"language-csharp",children:"services.AddKafka(\n    kafka => kafka\n        .SubscribeGlobalEvents(observers =>\n        {\n            observers.MessageConsumeStarted.Subscribe(eventContext =>\n            {\n                // Add your logic here\n            });\n        })\n"})}),"\n",(0,r.jsx)(s.h2,{id:"message-consume-completed-event",children:"Message Consume Completed Event"}),"\n",(0,r.jsx)(s.p,{children:"The Message Consume Completed Event signals the successful completion of message consumption. By subscribing to this event, you can track when messages have been successfully processed."}),"\n",(0,r.jsx)(s.admonition,{type:"info",children:(0,r.jsx)(s.p,{children:"Please note that the current event is not compatible with Batch Consume in the current version (v2). However, this limitation is expected to be addressed in future releases (v3+)."})}),"\n",(0,r.jsx)(s.pre,{children:(0,r.jsx)(s.code,{className:"language-csharp",children:"services.AddKafka(\n    kafka => kafka\n        .SubscribeGlobalEvents(observers =>\n        {\n            observers.MessageProduceCompleted.Subscribe(eventContext =>\n            {\n                // Add your logic here\n            });\n        })\n"})}),"\n",(0,r.jsx)(s.h2,{id:"message-consume-error-event",children:"Message Consume Error Event"}),"\n",(0,r.jsx)(s.p,{children:"If an error occurs during message consumption, the Message Consume Error Event is triggered. Subscribing to this event allows you to manage and respond to consumption errors."}),"\n",(0,r.jsx)(s.pre,{children:(0,r.jsx)(s.code,{className:"language-csharp",children:"services.AddKafka(\n    kafka => kafka\n        .SubscribeGlobalEvents(observers =>\n        {\n            observers.MessageConsumeError.Subscribe(eventContext =>\n            {\n                // Add your logic here\n            });\n        })\n"})})]})}function u(e={}){const{wrapper:s}={...(0,t.R)(),...e.components};return s?(0,r.jsx)(s,{...e,children:(0,r.jsx)(l,{...e})}):l(e)}},28453:(e,s,n)=>{n.d(s,{R:()=>a,x:()=>i});var r=n(96540);const t={},o=r.createContext(t);function a(e){const s=r.useContext(o);return r.useMemo((function(){return"function"==typeof e?e(s):{...s,...e}}),[s,e])}function i(e){let s;return s=e.disableParentContext?"function"==typeof e.components?e.components(t):e.components||t:a(e.components),r.createElement(o.Provider,{value:s},e.children)}}}]);