import React from "react";
import useDocusaurusContext from "@docusaurus/useDocusaurusContext";
import Layout from "@theme/Layout";

function Extension({title, description, link})
  {
    return (
      <div class="card-demo padding-top--md">
        <div class="card">
          <div class="card__header">
            <h3>{title}</h3>
          </div>
          <div class="card__body">
            <p>
              {description}
            </p>
            <a
              title={title}
              href={link}
            >
              {link}
            </a>
          </div>
        </div>
      </div>)
  }

export default function Extensions() {
  const ExtensionsList = [
    {
      title: 'KafkaFlow Retry Extensions',
      description: (
        <>
          KafkaFlow Retry is a .NET framework to implement easy resilience on consumers.
        </>
      ),
      link:'https://github.com/Farfetch/kafkaflow-retry-extensions'
    },
    {
      title: 'KafkaFlow MessagePack Serializer',
      description: (
        <>
          KafkaFlow MessagePack Serializer is an extension of KafkaFlow that use the MessagePack-CSharp library to optimize message sizes.
        </>
      ),
      link:'https://github.com/JotaDobleEse/kafkaflow-messagepack-serializer'
    },
    {
      title: 'KafkaFlow.MediatR',
      description: (
        <>
          An Extension that adds MediatR as a Middleware.
        </>
      ),
      link:'https://github.com/gsferreira/kafkaflow-mediatr'
    },
    {
      title: 'KafkaFlow.Contrib',
      description: (
        <>
          The project contains several extensions to implement features such as Transactional Outbox or Process Managers.
        </>
      ),
      link:'https://github.com/AlexeyRaga/kafkaflow-contrib'
    },
  ];

  const { siteConfig } = useDocusaurusContext();

  return (
    <Layout
      title={`Extensions | ${siteConfig.title}`}
      description="KafkaFlow Extensions developed by the community"
    >
      <main>
        <div class="container padding-top--md padding-bottom--lg">
          <div class="row">
            <div class="col">
              <article>
                <h1>Extensions</h1>
                <p>Extensions developed by the community.</p>
                <div class="alert alert--info" role="alert">
                  If you are an extension author and want to have your extension
                  listed, please let us know{" "}
                  <a href="https://github.com/Farfetch/kafkaflow/issues">
                    here
                  </a>
                  .
                </div>

                <div class="row">
                  <div class="col">
                    {ExtensionsList.map((props, idx) => (
                      <Extension key={idx} {...props} />
                    ))}
                  </div>
                </div>
              </article>
            </div>
          </div>
        </div>
      </main>
    </Layout>
  );
}
