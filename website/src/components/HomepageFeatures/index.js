import React from 'react';
import clsx from 'clsx';
import styles from './styles.module.css';

const FeatureList = [
  {
    title: 'Easy to Use',
    description: (
      <>
        KafkaFlow was designed to be easy to use and to quickly develop applications that work with Apache Kafka.
      </>
    ),
  },
  {
    title: 'Focus on What Matters',
    description: (
      <>
        KafkaFlow lets you focus on your logic, ignoring hosting and infrastructure details.
      </>
    ),
  },
  {
    title: 'Highly Extensible',
    description: (
      <>
        Extend KafkaFlow with new Serializers, Schema Registries, Dependency Injection containers, and many more.
      </>
    ),
  },
];

function Feature({title, description}) {
  return (
    <div className={clsx('col col--4')}>
      <div className="text--center padding-horiz--md">
        <h3>{title}</h3>
        <p>{description}</p>
      </div>
    </div>
  );
}

export default function HomepageFeatures() {
  return (
    <section className={styles.features}>
      <div className="container">
        <div className="row">
          {FeatureList.map((props, idx) => (
            <Feature key={idx} {...props} />
          ))}
        </div>
      </div>
    </section>
  );
}
