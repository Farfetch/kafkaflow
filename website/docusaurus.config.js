// @ts-check
// Note: type annotations allow type checking and IDEs autocompletion

const lightCodeTheme = require('prism-react-renderer/themes/github');
const darkCodeTheme = require('prism-react-renderer/themes/dracula');

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: 'KafkaFlow',
  tagline: 'A .NET framework to create Kafka-based applications, simple to use and extend.',
  url: 'https://farfetch.github.io/',
  baseUrl: '/kafkaflow/',
  onBrokenLinks: 'throw',
  onBrokenMarkdownLinks: 'warn',
  favicon: 'img/favicon.ico',

  // GitHub pages deployment config.
  // If you aren't using GitHub pages, you don't need these.
  organizationName: 'FARFETCH', // Usually your GitHub org/user name.
  projectName: 'KafkaFlow', // Usually your repo name.

  // Even if you don't use internalization, you can use this field to set useful
  // metadata like html lang. For example, if your site is Chinese, you may want
  // to replace "en" with "zh-Hans".
  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  presets: [
    [
      'classic',
      /** @type {import('@docusaurus/preset-classic').Options} */
      ({
        docs: {
          sidebarPath: require.resolve('./sidebars.js'),
          // Please change this to your repo.
          // Remove this to remove the "edit this page" links.
          editUrl:
            'https://github.com/farfetch/kafkaflow/tree/master/website/',
        },
        theme: {
          customCss: require.resolve('./src/css/custom.css'),
        },
        blog: false
      }),
    ],
  ],

  themeConfig:
    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
    ({
      colorMode: {
        defaultMode: 'light',
        disableSwitch: true,
      },
      navbar: {
        title: 'KafkaFlow',
        items: [
          {
            type: 'doc',
            docId: 'introduction',
            position: 'right',
            label: 'Docs',
          },
          {
            href: 'https://github.com/farfetch/kafkaflow',
            label: 'GitHub',
            position: 'right',
          },
        ],
      },
      footer: {
        style: 'dark',
        links: [
          {
            title: 'Docs',
            items: [
              {
                label: 'Introduction',
                to: '/docs',
              },
              {
                label: 'Getting Started',
                to: '/docs/category/getting-started',
              },
              {
                label: 'Guides',
                to: '/docs/category/guides',
              },
            ],
          },
          {
            title: 'Community',
            items: [
              {
                label: 'Stack Overflow',
                href: 'https://stackoverflow.com/questions/tagged/kafkaflow',
              },
              {
                label: 'Slack',
                href: 'https://join.slack.com/t/kafkaflow/shared_invite/zt-puihrtcl-NnnylPZloAiVlQfsw~RD6Q',
              },
            ],
          },
          {
            title: 'More',
            items: [
              {
                label: 'FARFETCH Blog',
                to: 'https://farfetchtechblog.com',
              },
              {
                label: 'GitHub',
                href: 'https://github.com/farfetch/kafkaflow',
              },
            ],
          },
        ],
        copyright: `Copyright © ${new Date().getFullYear()} FARFETCH UK Limited. Built with Docusaurus.`,
      },
      prism: {
        theme: lightCodeTheme,
        darkTheme: darkCodeTheme,
        additionalLanguages: ['csharp']
      },
    }),
};

module.exports = config;
