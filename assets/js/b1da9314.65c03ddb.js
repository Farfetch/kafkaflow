"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[734],{17193:(e,n,r)=>{r.r(n),r.d(n,{assets:()=>a,contentTitle:()=>t,default:()=>l,frontMatter:()=>c,metadata:()=>d,toc:()=>i});var o=r(74848),s=r(28453);const c={sidebar_position:2},t="Producers",d={id:"guides/producers",title:"Producers",description:"In this section, we will learn how to add and configure a Producer on KafkaFlow.",source:"@site/docs/guides/producers.md",sourceDirName:"guides",slug:"/guides/producers",permalink:"/kafkaflow/docs/guides/producers",draft:!1,unlisted:!1,editUrl:"https://github.com/farfetch/kafkaflow/tree/master/website/docs/guides/producers.md",tags:[],version:"current",sidebarPosition:2,frontMatter:{sidebar_position:2},sidebar:"tutorialSidebar",previous:{title:"Configuration",permalink:"/kafkaflow/docs/guides/configuration"},next:{title:"Consumers",permalink:"/kafkaflow/docs/category/consumers"}},a={},i=[{value:"Name-based producers",id:"name-based-producers",level:2},{value:"Type-based producers",id:"type-based-producers",level:2},{value:"How to produce a message to a given topic",id:"how-to-produce-a-message-to-a-given-topic",level:2},{value:"How to produce a message without Message Key",id:"how-to-produce-a-message-without-message-key",level:2},{value:"How to configure ACKS when publishing a message",id:"how-to-configure-acks-when-publishing-a-message",level:2},{value:"How to customize compression",id:"how-to-customize-compression",level:2}];function u(e){const n={a:"a",admonition:"admonition",code:"code",h1:"h1",h2:"h2",li:"li",p:"p",pre:"pre",table:"table",tbody:"tbody",td:"td",th:"th",thead:"thead",tr:"tr",ul:"ul",...(0,s.R)(),...e.components};return(0,o.jsxs)(o.Fragment,{children:[(0,o.jsx)(n.h1,{id:"producers",children:"Producers"}),"\n",(0,o.jsx)(n.p,{children:"In this section, we will learn how to add and configure a Producer on KafkaFlow."}),"\n",(0,o.jsxs)(n.p,{children:["To produce messages using KafkaFlow, Producers need to be configured in the application ",(0,o.jsx)(n.a,{href:"configuration",children:"configuration"}),"."]}),"\n",(0,o.jsxs)(n.p,{children:["The producers also support ",(0,o.jsx)(n.a,{href:"middlewares",children:"Middlewares"}),"."]}),"\n",(0,o.jsx)(n.p,{children:"You have two ways to configure the producers:"}),"\n",(0,o.jsxs)(n.ul,{children:["\n",(0,o.jsx)(n.li,{children:(0,o.jsx)(n.a,{href:"#named-producers",children:"Name-based producer"})}),"\n",(0,o.jsx)(n.li,{children:(0,o.jsx)(n.a,{href:"#type-based-producers",children:"Type-based producer"})}),"\n"]}),"\n",(0,o.jsx)(n.admonition,{type:"tip",children:(0,o.jsxs)(n.p,{children:["It's highly recommended to read ",(0,o.jsx)(n.a,{href:"https://github.com/confluentinc/confluent-kafka-dotnet/wiki/Producer",children:"Confluent Producer documentation"})," for better practices when producing messages."]})}),"\n",(0,o.jsx)(n.h2,{id:"name-based-producers",children:"Name-based producers"}),"\n",(0,o.jsx)(n.p,{children:"Uses a name to bind the configuration to the producer instance.\nUse the name as a key to access the Producer when you want to produce a message."}),"\n",(0,o.jsx)(n.pre,{children:(0,o.jsx)(n.code,{className:"language-csharp",children:'using KafkaFlow;\nusing KafkaFlow.Producers;\nusing Microsoft.Extensions.DependencyInjection;\n\nservices.AddKafka(kafka => kafka\n    .AddCluster(cluster => cluster\n        .WithBrokers(new[] { "localhost:9092" })\n        .AddProducer(\n            "product-events", //the producer name\n            producer => \n                producer\n        )\n    )\n);\n\npublic class ProductService : IProductService\n{\n    private readonly IProducerAccessor _producers;\n\n    public ProductService(IProducerAccessor producers)\n    {\n        _producers = producers;\n    }\n\n    public async Task CreateProduct(Product product) =>\n        await _producers["product-events"]\n            .ProduceAsync(product.Id.ToString(), product);     \n}\n'})}),"\n",(0,o.jsx)(n.h2,{id:"type-based-producers",children:"Type-based producers"}),"\n",(0,o.jsx)(n.p,{children:"Uses a class to bind the configuration to the producer instance, this is commonly used when you create a producer class to decouple the framework from your service classes."}),"\n",(0,o.jsxs)(n.p,{children:["For example, if you have a ",(0,o.jsx)(n.code,{children:"ProductEventsProducer"})," in your app, you can use this class when configuring the producer to bind the configuration with the instance of ",(0,o.jsx)(n.code,{children:"IMessageProducer<ProductEventsProducer>"}),"."]}),"\n",(0,o.jsx)(n.pre,{children:(0,o.jsx)(n.code,{className:"language-csharp",children:'using KafkaFlow;\nusing KafkaFlow.Producers;\nusing Microsoft.Extensions.DependencyInjection;\n\nvar services = new ServiceCollection();\n\nservices.AddKafka(kafka => kafka\n    .AddCluster(cluster => cluster\n        .WithBrokers(new[] { "localhost:9092" })\n        .AddProducer<ProductEventsProducer>(\n            producer => \n                producer\n                ...\n            )\n    )\n);\n\npublic class ProductEventsProducer : IProductEventsProducer\n{\n    private readonly IMessageProducer<ProductEventsProducer> _producer;\n\n    public ProductEventsProducer(IMessageProducer<ProductEventsProducer> producer)\n    {\n        _producer = producer;\n    }\n\n    public Task ProduceAsync(Product product) =>\n        _producer\n            .ProduceAsync(product.Id.ToString(), product);\n}\n'})}),"\n",(0,o.jsx)(n.h2,{id:"how-to-produce-a-message-to-a-given-topic",children:"How to produce a message to a given topic"}),"\n",(0,o.jsx)(n.p,{children:"There are two ways to specify the destination Topic of a message."}),"\n",(0,o.jsx)(n.p,{children:"You can specify it as the first argument when the Produce method is invoked, as shown in the following example:"}),"\n",(0,o.jsx)(n.pre,{children:(0,o.jsx)(n.code,{className:"language-csharp",children:'await _producers["product-events"]\n    .ProduceAsync("products-topic", product.Id.ToString(), product);\n'})}),"\n",(0,o.jsx)(n.p,{children:"You can also set the Default Topic that a Producer should produce to.\nYou can do that, on the producer configuration."}),"\n",(0,o.jsx)(n.pre,{children:(0,o.jsx)(n.code,{className:"language-csharp",children:'using KafkaFlow;\nusing KafkaFlow.Producers;\nusing Microsoft.Extensions.DependencyInjection;\n\nservices.AddKafka(kafka => kafka\n    .AddCluster(cluster => cluster\n        .WithBrokers(new[] { "localhost:9092" })\n        .AddProducer(\n            "product-events",\n            producer => \n                producer\n                    .DefaultTopic("products-topic")\n        )\n    )\n);\n'})}),"\n",(0,o.jsx)(n.h2,{id:"how-to-produce-a-message-without-message-key",children:"How to produce a message without Message Key"}),"\n",(0,o.jsxs)(n.p,{children:["You can send the message key argument as ",(0,o.jsx)(n.code,{children:"null"})," when the Produce method is invoked, as shown in the following example:"]}),"\n",(0,o.jsx)(n.pre,{children:(0,o.jsx)(n.code,{className:"language-csharp",children:"await producer.ProduceAsync(null, product);\n"})}),"\n",(0,o.jsx)(n.h2,{id:"how-to-configure-acks-when-publishing-a-message",children:"How to configure ACKS when publishing a message"}),"\n",(0,o.jsx)(n.p,{children:"An Ack is an acknowledgment that the producer receives from the broker to ensure that the message has been successfully committed."}),"\n",(0,o.jsxs)(n.p,{children:["The following table establishes the mapping between KafkaFlow and Kafka. You can find ",(0,o.jsx)(n.a,{href:"https://docs.confluent.io/platform/current/installation/configuration/producer-configs.html#producerconfigs_acks",children:"here"})," the meaning of each of those values."]}),"\n",(0,o.jsxs)(n.table,{children:[(0,o.jsx)(n.thead,{children:(0,o.jsxs)(n.tr,{children:[(0,o.jsx)(n.th,{children:"KafkaFlow"}),(0,o.jsx)(n.th,{children:"Kafka"})]})}),(0,o.jsxs)(n.tbody,{children:[(0,o.jsxs)(n.tr,{children:[(0,o.jsx)(n.td,{children:"Acks.None"}),(0,o.jsx)(n.td,{children:(0,o.jsx)(n.code,{children:"acks=0"})})]}),(0,o.jsxs)(n.tr,{children:[(0,o.jsx)(n.td,{children:"Acks.Leader"}),(0,o.jsx)(n.td,{children:(0,o.jsx)(n.code,{children:"acks=1"})})]}),(0,o.jsxs)(n.tr,{children:[(0,o.jsx)(n.td,{children:"Acks.All"}),(0,o.jsx)(n.td,{children:(0,o.jsx)(n.code,{children:"acks=all"})})]})]})]}),"\n",(0,o.jsx)(n.pre,{children:(0,o.jsx)(n.code,{className:"language-csharp",children:'using KafkaFlow;\nusing KafkaFlow.Producers;\nusing Microsoft.Extensions.DependencyInjection;\n\nservices.AddKafka(kafka => kafka\n    .AddCluster(cluster => cluster\n        .WithBrokers(new[] { "localhost:9092" })\n        .AddProducer(\n            "product-events",\n            producer => \n                producer\n                    .WithAcks(Acks.Leader)\n        )\n    )\n);\n'})}),"\n",(0,o.jsx)(n.h2,{id:"how-to-customize-compression",children:"How to customize compression"}),"\n",(0,o.jsxs)(n.p,{children:["You can find more information in the ",(0,o.jsx)(n.a,{href:"compression",children:"Compression guide"}),"."]})]})}function l(e={}){const{wrapper:n}={...(0,s.R)(),...e.components};return n?(0,o.jsx)(n,{...e,children:(0,o.jsx)(u,{...e})}):u(e)}},28453:(e,n,r)=>{r.d(n,{R:()=>t,x:()=>d});var o=r(96540);const s={},c=o.createContext(s);function t(e){const n=o.useContext(c);return o.useMemo((function(){return"function"==typeof e?e(n):{...n,...e}}),[n,e])}function d(e){let n;return n=e.disableParentContext?"function"==typeof e.components?e.components(s):e.components||s:t(e.components),o.createElement(c.Provider,{value:n},e.children)}}}]);