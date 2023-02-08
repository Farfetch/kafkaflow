"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[8559],{3905:(e,t,r)=>{r.d(t,{Zo:()=>c,kt:()=>m});var a=r(7294);function n(e,t,r){return t in e?Object.defineProperty(e,t,{value:r,enumerable:!0,configurable:!0,writable:!0}):e[t]=r,e}function o(e,t){var r=Object.keys(e);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);t&&(a=a.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),r.push.apply(r,a)}return r}function s(e){for(var t=1;t<arguments.length;t++){var r=null!=arguments[t]?arguments[t]:{};t%2?o(Object(r),!0).forEach((function(t){n(e,t,r[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(r)):o(Object(r)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(r,t))}))}return e}function l(e,t){if(null==e)return{};var r,a,n=function(e,t){if(null==e)return{};var r,a,n={},o=Object.keys(e);for(a=0;a<o.length;a++)r=o[a],t.indexOf(r)>=0||(n[r]=e[r]);return n}(e,t);if(Object.getOwnPropertySymbols){var o=Object.getOwnPropertySymbols(e);for(a=0;a<o.length;a++)r=o[a],t.indexOf(r)>=0||Object.prototype.propertyIsEnumerable.call(e,r)&&(n[r]=e[r])}return n}var i=a.createContext({}),u=function(e){var t=a.useContext(i),r=t;return e&&(r="function"==typeof e?e(t):s(s({},t),e)),r},c=function(e){var t=u(e.components);return a.createElement(i.Provider,{value:t},e.children)},p={inlineCode:"code",wrapper:function(e){var t=e.children;return a.createElement(a.Fragment,{},t)}},f=a.forwardRef((function(e,t){var r=e.components,n=e.mdxType,o=e.originalType,i=e.parentName,c=l(e,["components","mdxType","originalType","parentName"]),f=u(r),m=n,d=f["".concat(i,".").concat(m)]||f[m]||p[m]||o;return r?a.createElement(d,s(s({ref:t},c),{},{components:r})):a.createElement(d,s({ref:t},c))}));function m(e,t){var r=arguments,n=t&&t.mdxType;if("string"==typeof e||n){var o=r.length,s=new Array(o);s[0]=f;var l={};for(var i in t)hasOwnProperty.call(t,i)&&(l[i]=t[i]);l.originalType=e,l.mdxType="string"==typeof e?e:n,s[1]=l;for(var u=2;u<o;u++)s[u]=r[u];return a.createElement.apply(null,s)}return a.createElement.apply(null,r)}f.displayName="MDXCreateElement"},9160:(e,t,r)=>{r.r(t),r.d(t,{assets:()=>i,contentTitle:()=>s,default:()=>p,frontMatter:()=>o,metadata:()=>l,toc:()=>u});var a=r(7462),n=(r(7294),r(3905));const o={},s="IMessageConsumer.Pause method",l={unversionedId:"reference/KafkaFlow/KafkaFlow.Consumers/IMessageConsumer/Pause",id:"reference/KafkaFlow/KafkaFlow.Consumers/IMessageConsumer/Pause",title:"IMessageConsumer.Pause method",description:"Pause consumption for the provided list of partitions.",source:"@site/docs/reference/KafkaFlow/KafkaFlow.Consumers/IMessageConsumer/Pause.md",sourceDirName:"reference/KafkaFlow/KafkaFlow.Consumers/IMessageConsumer",slug:"/reference/KafkaFlow/KafkaFlow.Consumers/IMessageConsumer/Pause",permalink:"/kafkaflow/docs/reference/KafkaFlow/KafkaFlow.Consumers/IMessageConsumer/Pause",draft:!1,editUrl:"https://github.com/farfetch/kafkaflow/tree/master/website/docs/reference/KafkaFlow/KafkaFlow.Consumers/IMessageConsumer/Pause.md",tags:[],version:"current",frontMatter:{},sidebar:"tutorialSidebar",previous:{title:"IMessageConsumer.OverrideOffsetsAndRestartAsync method",permalink:"/kafkaflow/docs/reference/KafkaFlow/KafkaFlow.Consumers/IMessageConsumer/OverrideOffsetsAndRestartAsync"},next:{title:"IMessageConsumer.PausedPartitions property",permalink:"/kafkaflow/docs/reference/KafkaFlow/KafkaFlow.Consumers/IMessageConsumer/PausedPartitions"}},i={},u=[{value:"Exceptions",id:"exceptions",level:2},{value:"See Also",id:"see-also",level:2}],c={toc:u};function p(e){let{components:t,...r}=e;return(0,n.kt)("wrapper",(0,a.Z)({},c,r,{components:t,mdxType:"MDXLayout"}),(0,n.kt)("h1",{id:"imessageconsumerpause-method"},"IMessageConsumer.Pause method"),(0,n.kt)("p",null,"Pause consumption for the provided list of partitions."),(0,n.kt)("pre",null,(0,n.kt)("code",{parentName:"pre",className:"language-csharp"},"public void Pause(IReadOnlyCollection<TopicPartition> partitions)\n")),(0,n.kt)("table",null,(0,n.kt)("thead",{parentName:"table"},(0,n.kt)("tr",{parentName:"thead"},(0,n.kt)("th",{parentName:"tr",align:null},"parameter"),(0,n.kt)("th",{parentName:"tr",align:null},"description"))),(0,n.kt)("tbody",{parentName:"table"},(0,n.kt)("tr",{parentName:"tbody"},(0,n.kt)("td",{parentName:"tr",align:null},"partitions"),(0,n.kt)("td",{parentName:"tr",align:null},"The partitions to pause consumption of.")))),(0,n.kt)("h2",{id:"exceptions"},"Exceptions"),(0,n.kt)("table",null,(0,n.kt)("thead",{parentName:"table"},(0,n.kt)("tr",{parentName:"thead"},(0,n.kt)("th",{parentName:"tr",align:null},"exception"),(0,n.kt)("th",{parentName:"tr",align:null},"condition"))),(0,n.kt)("tbody",{parentName:"table"},(0,n.kt)("tr",{parentName:"tbody"},(0,n.kt)("td",{parentName:"tr",align:null},"KafkaException"),(0,n.kt)("td",{parentName:"tr",align:null},"Thrown if the request failed.")),(0,n.kt)("tr",{parentName:"tbody"},(0,n.kt)("td",{parentName:"tr",align:null},"TopicPartitionException"),(0,n.kt)("td",{parentName:"tr",align:null},"Per partition success or error.")))),(0,n.kt)("h2",{id:"see-also"},"See Also"),(0,n.kt)("ul",null,(0,n.kt)("li",{parentName:"ul"},"interface\xa0",(0,n.kt)("a",{parentName:"li",href:"/kafkaflow/docs/reference/KafkaFlow/KafkaFlow.Consumers/IMessageConsumer/"},"IMessageConsumer")),(0,n.kt)("li",{parentName:"ul"},"namespace\xa0",(0,n.kt)("a",{parentName:"li",href:"/kafkaflow/docs/reference/KafkaFlow/"},"KafkaFlow.Consumers"))))}p.isMDXComponent=!0}}]);