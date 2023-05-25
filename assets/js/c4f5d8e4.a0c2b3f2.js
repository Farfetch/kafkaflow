/*! For license information please see c4f5d8e4.a0c2b3f2.js.LICENSE.txt */
"use strict";(self.webpackChunkwebsite=self.webpackChunkwebsite||[]).push([[4195],{9911:(e,t,a)=>{a.r(t),a.d(t,{default:()=>v});var n=a(7294),l=a(6010),r=a(9960),c=a(4996),i=a(2263),o=a(7961),s=a(7462);const m="features_t9lD",u=[{title:"Easy to Use",description:n.createElement(n.Fragment,null,"KafkaFlow was designed to be easy to use and to quickly develop applications that work with Apache Kafka.")},{title:"Focus on What Matters",description:n.createElement(n.Fragment,null,"KafkaFlow lets you focus on your logic, ignoring hosting and infrastructure details.")},{title:"Highly Extensible",description:n.createElement(n.Fragment,null,"Extend KafkaFlow with new Serializers, Schema Registries, Dependency Injection containers, and many more.")}];function p(e){let{title:t,description:a}=e;return n.createElement("div",{className:(0,l.Z)("col col--4")},n.createElement("div",{className:"text--center padding-horiz--md"},n.createElement("h3",null,t),n.createElement("p",null,a)))}function d(){return n.createElement("section",{className:m},n.createElement("div",{className:"container"},n.createElement("div",{className:"row"},u.map(((e,t)=>n.createElement(p,(0,s.Z)({key:t},e)))))))}var h=function(){return h=Object.assign||function(e){for(var t,a=1,n=arguments.length;a<n;a++)for(var l in t=arguments[a])Object.prototype.hasOwnProperty.call(t,l)&&(e[l]=t[l]);return e},h.apply(this,arguments)};function E(e){var t=n.useState(!1),a=t[0],l=t[1],r=n.useState(!1),c=r[0],i=r[1],o=encodeURIComponent(e.id),s="string"==typeof e.playlistCoverId?encodeURIComponent(e.playlistCoverId):null,m=e.title,u=e.poster||"hqdefault",p="&"+e.params||0,d=e.muted?"&mute=1":"",E=e.announce||"Watch",f=e.webp?"webp":"jpg",g=e.webp?"vi_webp":"vi",w=e.thumbnail||(e.playlist?"https://i.ytimg.com/"+g+"/"+s+"/"+u+"."+f:"https://i.ytimg.com/"+g+"/"+o+"/"+u+"."+f),y=e.noCookie;y=e.cookie?"https://www.youtube.com":"https://www.youtube-nocookie.com";var b=e.playlist?y+"/embed/videoseries?autoplay=1"+d+"&list="+o+p:y+"/embed/"+o+"?autoplay=1&state=1"+d+p,v=e.activatedClass||"lyt-activated",k=e.adNetwork||!1,N=e.aspectHeight||9,C=e.aspectWidth||16,F=e.iframeClass||"",Z=e.playerClass||"lty-playbtn",I=e.wrapperClass||"yt-lite",_=e.onIframeAdded||function(){},x=e.rel?"prefetch":"preload";return n.useEffect((function(){c&&_()}),[c]),n.createElement(n.Fragment,null,n.createElement("link",{rel:x,href:w,as:"image"}),n.createElement(n.Fragment,null,a&&n.createElement(n.Fragment,null,n.createElement("link",{rel:"preconnect",href:y}),n.createElement("link",{rel:"preconnect",href:"https://www.google.com"}),k&&n.createElement(n.Fragment,null,n.createElement("link",{rel:"preconnect",href:"https://static.doubleclick.net"}),n.createElement("link",{rel:"preconnect",href:"https://googleads.g.doubleclick.net"})))),n.createElement("article",{onPointerOver:function(){a||l(!0)},onClick:function(){c||i(!0)},className:I+" "+(c?v:""),"data-title":m,style:h({backgroundImage:"url("+w+")"},{"--aspect-ratio":N/C*100+"%"})},n.createElement("button",{type:"button",className:Z,"aria-label":E+" "+m}),c&&n.createElement("iframe",{className:F,title:m,width:"560",height:"315",frameBorder:"0",allow:"accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture",allowFullScreen:!0,src:b})))}const f="heroBanner_qdFl",g="buttons_AeoN",w="videoContainer_ViX2";function y(){const{siteConfig:e}=(0,i.Z)();return n.createElement("header",{className:(0,l.Z)("hero",f)},n.createElement("div",{className:"container"},n.createElement("img",{alt:e.title,src:(0,c.Z)("/img/logo.svg"),width:"512"}),n.createElement("p",{className:"hero__subtitle"},e.tagline),n.createElement("div",{className:g},n.createElement(r.Z,{className:"button button--secondary button--lg",to:"/docs/"},"Get Started"))))}function b(){return n.createElement("div",{className:"container text--center margin-bottom--xl"},n.createElement("div",{className:"row"},n.createElement("div",{className:"col"},n.createElement("h2",null,"Check it out in a intro video"),n.createElement("div",{className:w},n.createElement(E,{id:"4e18DZkf-m0",params:"autoplay=1&autohide=1&showinfo=0&rel=0",title:"A BETTER Way to Kafka Event Diven Applications with C#",poster:"maxresdefault",webp:!0})))))}function v(){const{siteConfig:e}=(0,i.Z)();return n.createElement(o.Z,{title:`Hello from ${e.title}`,description:"Description will go into a meta tag in <head />"},n.createElement(y,null),n.createElement("main",null,n.createElement(d,null),n.createElement(b,null)))}}}]);