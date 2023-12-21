import React from "react";
import useDocusaurusContext from "@docusaurus/useDocusaurusContext";
import Layout from "@theme/Layout";
import { Resource } from "../../components/Community/Resource/Index";

const resources = [
  {
    title:
      "[PORTUGUESE] KafkaFlow, An open-source Kafka framework for .NET : Live Demo",
    description: null,
    link: "https://www.youtube.com/watch?v=XJCoI38KK7o",
    type: "Talk",
    date: "2021-12-15",
    authors: [
      {
        name: "Filipe Esch",
        url: "https://github.com/filipeesch",
      },
    ],
  },
  {
    title: "A BETTER Way to Kafka Event Diven Applications with C#",
    description: null,
    link: "https://www.youtube.com/watch?v=4e18DZkf-m0",
    type: "Video",
    date: "2023-01-24",
    authors: [
      {
        name: "Gui Ferreira",
        url: "https://guiferreira.me",
      },
    ],
  },
  {
    title:
      "Apache Kafka in 1 hour for C# Developers - Guilherme Ferreira - NDC London 2023",
    description: null,
    link: "https://www.youtube.com/watch?v=4xpjlqIlfY8",
    type: "Talk",
    date: "2023-05-20",
    authors: [
      {
        name: "Gui Ferreira",
        url: "https://guiferreira.me",
      },
    ],
  },
  {
    title: "3 KafkaFlow Features Hard to Ignore",
    description: null,
    link: "https://www.youtube.com/watch?v=v-aFkzlBVpE",
    type: "Video",
    date: "2023-06-20",
    authors: [
      {
        name: "Gui Ferreira",
        url: "https://guiferreira.me",
      },
    ],
  },
  {
    title: "How KafkaFlow Supercharged FARFETCH's Event-Driven Architecture",
    description: null,
    link: "https://www.farfetchtechblog.com/en/blog/post/how-kafkaflow-supercharged-farfetch-s-event-driven-architecture/",
    type: "Article",
    date: "2023-08-25",
    authors: [
      {
        name: "Filipe Esch",
        url: "https://github.com/filipeesch",
      },
      {
        name: "Gui Ferreira",
        url: "https://guiferreira.me",
      },
    ],
  },
  {
    title: "Building Kafka Event-Driven Applications with KafkaFlow",
    description: null,
    link: "https://www.infoq.com/articles/kafkaflow-dotnet-framework/",
    type: "Article",
    date: "2023-09-08",
    authors: [
      {
        name: "Gui Ferreira",
        url: "https://guiferreira.me",
      },
    ],
  },
  {
    title:
      "Apache Kafka in 1 hour for C# Developers - Guilherme Ferreira - Copenhagen DevFest 2023",
    description: null,
    link: "https://www.youtube.com/watch?v=E07CGvGVal8",
    type: "Talk",
    date: "2023-11-08",
    authors: [
      {
        name: "Gui Ferreira",
        url: "https://guiferreira.me",
      },
    ],
  },
  {
    title:
      "Instrumenting .NET Kafka Clients with OpenTelemetry and KafkaFlow",
    description: null,
    link: "https://youtu.be/HOivyibM-04?si=Hk7Mif1TnDJHskyb",
    type: "Video",
    date: "2023-12-05",
    authors: [
      {
        name: "Gui Ferreira",
        url: "https://guiferreira.me",
      },
    ],
  },
  {
    title:
      "How to Dynamically Subscribe to Kafka Topics in .NET",
    description: null,
    link: "https://youtu.be/18VMi6MgCSQ?si=koUDdESAbGcQ0FUU",
    type: "Video",
    date: "2023-12-12",
    authors: [
      {
        name: "Gui Ferreira",
        url: "https://guiferreira.me",
      },
    ],
  },
  {
    title:
      "A BETTER Way to Kafka Event-Driven Applications with C#",
    description: null,
    link: "https://guiferreira.me/archive/2023/a-better-way-to-kafka-event-driven-applications-with-csharp/",
    type: "Article",
    date: "2023-12-13",
    authors: [
      {
        name: "Gui Ferreira",
        url: "https://guiferreira.me",
      },
    ],
  },
  {
    title:
      "3 KafkaFlow Features Hard to Ignore",
    description: null,
    link: "https://guiferreira.me/archive/2023/3-kafkaflow-features-hard-to-ignore/",
    type: "Article",
    date: "2023-12-19",
    authors: [
      {
        name: "Gui Ferreira",
        url: "https://guiferreira.me",
      },
    ],
  },
];

export default function Community() {
  const { siteConfig } = useDocusaurusContext();

  return (
    <Layout
      title={`Community | ${siteConfig.title}`}
      description="KafkaFlow resources built by the community"
    >
      <main>
        <div class="container padding-top--md padding-bottom--lg">
          <div class="row">
            <div class="col">
              <article>
                <h1>Community</h1>
                <p>Resources built by the community.</p>
                <div class="alert alert--info" role="alert">
                  If you have any resource (blog post, talk, podcast, etc.)
                  where KafkaFlow is mentioned and you want to see it listed
                  here, please let us know{" "}
                  <a href="https://github.com/Farfetch/kafkaflow/issues">
                    (here)
                  </a>
                  .
                </div>

                <div class="row">
                  <div class="col">
                    {resources.map((props, idx) => (
                      <Resource key={idx} {...props} />
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
