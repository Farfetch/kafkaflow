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
