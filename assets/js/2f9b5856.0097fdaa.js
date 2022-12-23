"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[1157],{3905:(e,t,r)=>{r.d(t,{Zo:()=>m,kt:()=>p});var a=r(7294);function n(e,t,r){return t in e?Object.defineProperty(e,t,{value:r,enumerable:!0,configurable:!0,writable:!0}):e[t]=r,e}function o(e,t){var r=Object.keys(e);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);t&&(a=a.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),r.push.apply(r,a)}return r}function s(e){for(var t=1;t<arguments.length;t++){var r=null!=arguments[t]?arguments[t]:{};t%2?o(Object(r),!0).forEach((function(t){n(e,t,r[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(r)):o(Object(r)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(r,t))}))}return e}function c(e,t){if(null==e)return{};var r,a,n=function(e,t){if(null==e)return{};var r,a,n={},o=Object.keys(e);for(a=0;a<o.length;a++)r=o[a],t.indexOf(r)>=0||(n[r]=e[r]);return n}(e,t);if(Object.getOwnPropertySymbols){var o=Object.getOwnPropertySymbols(e);for(a=0;a<o.length;a++)r=o[a],t.indexOf(r)>=0||Object.prototype.propertyIsEnumerable.call(e,r)&&(n[r]=e[r])}return n}var l=a.createContext({}),i=function(e){var t=a.useContext(l),r=t;return e&&(r="function"==typeof e?e(t):s(s({},t),e)),r},m=function(e){var t=i(e.components);return a.createElement(l.Provider,{value:t},e.children)},y={inlineCode:"code",wrapper:function(e){var t=e.children;return a.createElement(a.Fragment,{},t)}},f=a.forwardRef((function(e,t){var r=e.components,n=e.mdxType,o=e.originalType,l=e.parentName,m=c(e,["components","mdxType","originalType","parentName"]),f=i(r),p=n,u=f["".concat(l,".").concat(p)]||f[p]||y[p]||o;return r?a.createElement(u,s(s({ref:t},m),{},{components:r})):a.createElement(u,s({ref:t},m))}));function p(e,t){var r=arguments,n=t&&t.mdxType;if("string"==typeof e||n){var o=r.length,s=new Array(o);s[0]=f;var c={};for(var l in t)hasOwnProperty.call(t,l)&&(c[l]=t[l]);c.originalType=e,c.mdxType="string"==typeof e?e:n,s[1]=c;for(var i=2;i<o;i++)s[i]=r[i];return a.createElement.apply(null,s)}return a.createElement.apply(null,r)}f.displayName="MDXCreateElement"},2818:(e,t,r)=>{r.r(t),r.d(t,{assets:()=>l,contentTitle:()=>s,default:()=>y,frontMatter:()=>o,metadata:()=>c,toc:()=>i});var a=r(7462),n=(r(7294),r(3905));const o={},s="IAsyncSchemaRegistryTypeNameResolver.ResolveAsync method",c={unversionedId:"reference/KafkaFlow.SchemaRegistry/KafkaFlow/IAsyncSchemaRegistryTypeNameResolver/ResolveAsync",id:"reference/KafkaFlow.SchemaRegistry/KafkaFlow/IAsyncSchemaRegistryTypeNameResolver/ResolveAsync",title:"IAsyncSchemaRegistryTypeNameResolver.ResolveAsync method",description:"Resolve the message type name of a schema",source:"@site/docs/reference/KafkaFlow.SchemaRegistry/KafkaFlow/IAsyncSchemaRegistryTypeNameResolver/ResolveAsync.md",sourceDirName:"reference/KafkaFlow.SchemaRegistry/KafkaFlow/IAsyncSchemaRegistryTypeNameResolver",slug:"/reference/KafkaFlow.SchemaRegistry/KafkaFlow/IAsyncSchemaRegistryTypeNameResolver/ResolveAsync",permalink:"/kafkaflow/docs/reference/KafkaFlow.SchemaRegistry/KafkaFlow/IAsyncSchemaRegistryTypeNameResolver/ResolveAsync",draft:!1,editUrl:"https://github.com/farfetch/kafkaflow/tree/master/website/docs/reference/KafkaFlow.SchemaRegistry/KafkaFlow/IAsyncSchemaRegistryTypeNameResolver/ResolveAsync.md",tags:[],version:"current",frontMatter:{},sidebar:"tutorialSidebar",previous:{title:"IAsyncSchemaRegistryTypeNameResolver interface",permalink:"/kafkaflow/docs/reference/KafkaFlow.SchemaRegistry/KafkaFlow/IAsyncSchemaRegistryTypeNameResolver/"},next:{title:"ISchemaRegistryTypeNameResolver interface",permalink:"/kafkaflow/docs/reference/KafkaFlow.SchemaRegistry/KafkaFlow/ISchemaRegistryTypeNameResolver/"}},l={},i=[{value:"See Also",id:"see-also",level:2}],m={toc:i};function y(e){let{components:t,...r}=e;return(0,n.kt)("wrapper",(0,a.Z)({},m,r,{components:t,mdxType:"MDXLayout"}),(0,n.kt)("h1",{id:"iasyncschemaregistrytypenameresolverresolveasync-method"},"IAsyncSchemaRegistryTypeNameResolver.ResolveAsync method"),(0,n.kt)("p",null,"Resolve the message type name of a schema"),(0,n.kt)("pre",null,(0,n.kt)("code",{parentName:"pre",className:"language-csharp"},"public Task<string> ResolveAsync(int schemaId)\n")),(0,n.kt)("table",null,(0,n.kt)("thead",{parentName:"table"},(0,n.kt)("tr",{parentName:"thead"},(0,n.kt)("th",{parentName:"tr",align:null},"parameter"),(0,n.kt)("th",{parentName:"tr",align:null},"description"))),(0,n.kt)("tbody",{parentName:"table"},(0,n.kt)("tr",{parentName:"tbody"},(0,n.kt)("td",{parentName:"tr",align:null},"schemaId"),(0,n.kt)("td",{parentName:"tr",align:null},"Identifier of the schema")))),(0,n.kt)("h2",{id:"see-also"},"See Also"),(0,n.kt)("ul",null,(0,n.kt)("li",{parentName:"ul"},"interface\xa0",(0,n.kt)("a",{parentName:"li",href:"/kafkaflow/docs/reference/KafkaFlow.SchemaRegistry/KafkaFlow/IAsyncSchemaRegistryTypeNameResolver/"},"IAsyncSchemaRegistryTypeNameResolver")),(0,n.kt)("li",{parentName:"ul"},"namespace\xa0",(0,n.kt)("a",{parentName:"li",href:"/kafkaflow/docs/reference/KafkaFlow.SchemaRegistry/"},"KafkaFlow"))))}y.isMDXComponent=!0}}]);