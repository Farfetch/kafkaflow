"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[8409],{28453:(e,r,s)=>{s.d(r,{R:()=>t,x:()=>d});var o=s(96540);const n={},i=o.createContext(n);function t(e){const r=o.useContext(i);return o.useMemo((function(){return"function"==typeof e?e(r):{...r,...e}}),[r,e])}function d(e){let r;return r=e.disableParentContext?"function"==typeof e.components?e.components(n):e.components||n:t(e.components),o.createElement(i.Provider,{value:r},e.children)}},60360:(e,r,s)=>{s.r(r),s.d(r,{assets:()=>a,contentTitle:()=>t,default:()=>h,frontMatter:()=>i,metadata:()=>d,toc:()=>l});var o=s(74848),n=s(28453);const i={},t="ConfigurationBuilderExtensions.AddCompressor<T> method (1 of 2)",d={id:"reference/KafkaFlow/KafkaFlow/ConfigurationBuilderExtensions/AddCompressor",title:"ConfigurationBuilderExtensions.AddCompressor&lt;T&gt; method (1 of 2)",description:"Registers a middleware to compress the message It is highly recommended to use the producer native compression ('WithCompression()' method) instead of using the compressor middleware",source:"@site/docs/reference/KafkaFlow/KafkaFlow/ConfigurationBuilderExtensions/AddCompressor.md",sourceDirName:"reference/KafkaFlow/KafkaFlow/ConfigurationBuilderExtensions",slug:"/reference/KafkaFlow/KafkaFlow/ConfigurationBuilderExtensions/AddCompressor",permalink:"/kafkaflow/docs/reference/KafkaFlow/KafkaFlow/ConfigurationBuilderExtensions/AddCompressor",draft:!1,unlisted:!1,editUrl:"https://github.com/farfetch/kafkaflow/tree/master/website/docs/reference/KafkaFlow/KafkaFlow/ConfigurationBuilderExtensions/AddCompressor.md",tags:[],version:"current",frontMatter:{},sidebar:"tutorialSidebar",previous:{title:"ConfigurationBuilderExtensions class",permalink:"/kafkaflow/docs/reference/KafkaFlow/KafkaFlow/ConfigurationBuilderExtensions/"},next:{title:"ConfigurationBuilderExtensions.AddDecompressor&lt;T&gt; method (1 of 2)",permalink:"/kafkaflow/docs/reference/KafkaFlow/KafkaFlow/ConfigurationBuilderExtensions/AddDecompressor"}},a={},l=[{value:"See Also",id:"see-also",level:2},{value:"See Also",id:"see-also-1",level:2}];function c(e){const r={a:"a",code:"code",h1:"h1",h2:"h2",hr:"hr",li:"li",p:"p",pre:"pre",table:"table",tbody:"tbody",td:"td",th:"th",thead:"thead",tr:"tr",ul:"ul",...(0,n.R)(),...e.components};return(0,o.jsxs)(o.Fragment,{children:[(0,o.jsx)(r.h1,{id:"configurationbuilderextensionsaddcompressort-method-1-of-2",children:"ConfigurationBuilderExtensions.AddCompressor<T> method (1 of 2)"}),"\n",(0,o.jsx)(r.p,{children:"Registers a middleware to compress the message It is highly recommended to use the producer native compression ('WithCompression()' method) instead of using the compressor middleware"}),"\n",(0,o.jsx)(r.pre,{children:(0,o.jsx)(r.code,{className:"language-csharp",children:"public static IProducerMiddlewareConfigurationBuilder AddCompressor<T>(\n    this IProducerMiddlewareConfigurationBuilder middlewares)\n    where T : class, ICompressor\n"})}),"\n",(0,o.jsxs)(r.table,{children:[(0,o.jsx)(r.thead,{children:(0,o.jsxs)(r.tr,{children:[(0,o.jsx)(r.th,{children:"parameter"}),(0,o.jsx)(r.th,{children:"description"})]})}),(0,o.jsxs)(r.tbody,{children:[(0,o.jsxs)(r.tr,{children:[(0,o.jsx)(r.td,{children:"T"}),(0,o.jsx)(r.td,{children:"The compressor type that implements ICompressor"})]}),(0,o.jsxs)(r.tr,{children:[(0,o.jsx)(r.td,{children:"middlewares"}),(0,o.jsx)(r.td,{children:"The middleware configuration builder"})]})]})]}),"\n",(0,o.jsx)(r.h2,{id:"see-also",children:"See Also"}),"\n",(0,o.jsxs)(r.ul,{children:["\n",(0,o.jsxs)(r.li,{children:["class\xa0",(0,o.jsx)(r.a,{href:"/kafkaflow/docs/reference/KafkaFlow/KafkaFlow/ConfigurationBuilderExtensions/",children:"ConfigurationBuilderExtensions"})]}),"\n",(0,o.jsxs)(r.li,{children:["namespace\xa0",(0,o.jsx)(r.a,{href:"/kafkaflow/docs/reference/KafkaFlow/",children:"KafkaFlow"})]}),"\n"]}),"\n",(0,o.jsx)(r.hr,{}),"\n",(0,o.jsx)(r.h1,{id:"configurationbuilderextensionsaddcompressort-method-2-of-2",children:"ConfigurationBuilderExtensions.AddCompressor<T> method (2 of 2)"}),"\n",(0,o.jsx)(r.p,{children:"Registers a middleware to compress the message It is highly recommended to use the producer native compression ('WithCompression()' method) instead of using the compressor middleware"}),"\n",(0,o.jsx)(r.pre,{children:(0,o.jsx)(r.code,{className:"language-csharp",children:"public static IProducerMiddlewareConfigurationBuilder AddCompressor<T>(\n    this IProducerMiddlewareConfigurationBuilder middlewares, Factory<T> factory)\n    where T : class, ICompressor\n"})}),"\n",(0,o.jsxs)(r.table,{children:[(0,o.jsx)(r.thead,{children:(0,o.jsxs)(r.tr,{children:[(0,o.jsx)(r.th,{children:"parameter"}),(0,o.jsx)(r.th,{children:"description"})]})}),(0,o.jsxs)(r.tbody,{children:[(0,o.jsxs)(r.tr,{children:[(0,o.jsx)(r.td,{children:"T"}),(0,o.jsx)(r.td,{children:"The compressor type that implements ICompressor"})]}),(0,o.jsxs)(r.tr,{children:[(0,o.jsx)(r.td,{children:"middlewares"}),(0,o.jsx)(r.td,{children:"The middleware configuration builder"})]}),(0,o.jsxs)(r.tr,{children:[(0,o.jsx)(r.td,{children:"factory"}),(0,o.jsx)(r.td,{children:"A factory to create the ICompressor instance"})]})]})]}),"\n",(0,o.jsx)(r.h2,{id:"see-also-1",children:"See Also"}),"\n",(0,o.jsxs)(r.ul,{children:["\n",(0,o.jsxs)(r.li,{children:["class\xa0",(0,o.jsx)(r.a,{href:"/kafkaflow/docs/reference/KafkaFlow/KafkaFlow/ConfigurationBuilderExtensions/",children:"ConfigurationBuilderExtensions"})]}),"\n",(0,o.jsxs)(r.li,{children:["namespace\xa0",(0,o.jsx)(r.a,{href:"/kafkaflow/docs/reference/KafkaFlow/",children:"KafkaFlow"})]}),"\n"]})]})}function h(e={}){const{wrapper:r}={...(0,n.R)(),...e.components};return r?(0,o.jsx)(r,{...e,children:(0,o.jsx)(c,{...e})}):c(e)}}}]);