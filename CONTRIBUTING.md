# Contributing

Hi there! We're thrilled that you'd like to contribute to this project. Your help is essential for keeping it great.

Please note that this project is released with a [Contributor Code of Conduct][code-of-conduct]. By participating in this project you agree to abide by its terms.

## Submitting a pull request

### Before opening a Pull Request

We recommend [opening an issue](https://github.com/Farfetch/kafkaflow/issues) before a substantial Pull Request if there isn’t [already an issue](https://github.com/Farfetch/kafkaflow/issues) for what you’d like to contribute. This helps facilitate a discussion before deciding on an implementation approach.

For some changes, such as typo fixes, documentation enhancements, or broken links, it may be suitable to open a small Pull Request by itself.

### How to open a Pull Request

1.  Check the issues or open a new one
2.  Fork this repository
3.  Create your feature branch: `git checkout -b my-new-feature`
4.  Commit your changes: `git commit -am 'feat: Add some feature'`
5.  Push to the branch: `git push origin my-new-feature`
6.  Submit a pull request linked to the issue 1.

Here are a few things you can do that will increase the likelihood of your pull request being accepted:

-   Follow the overall style of the project
-   Write tests
-   Keep your change as focused as possible. If there are multiple changes you would like to make that are not dependent upon each other, submit them as separate pull requests
-   Write [good commit messages](http://tbaggery.com/2008/04/19/a-note-about-git-commit-messages.html) following [conventional commits](https://www.conventionalcommits.org/en/v1.0.0/)
-   Open a pull request with a title following [conventional commits](https://www.conventionalcommits.org/en/v1.0.0/)

## Running Integration Tests

You can find a Makefile with steps for running a Kafka Cluster using docker on the repository root.

### To start cluster

Run command `make init_broker`

### To stop cluster

Run command `make shutdown_broker`

## Resources

-   [How to Contribute to Open Source](https://opensource.guide/how-to-contribute/)
-   [Using Pull Requests](https://help.github.com/articles/about-pull-requests/)
-   [GitHub Help](https://help.github.com)

### Disclaimer

By sending us your contributions, you are agreeing that your contribution is made subject to the terms of our [Contributor Ownership Statement](https://github.com/Farfetch/.github/blob/master/COS.md)

[code-of-conduct]: CODE_OF_CONDUCT.md
