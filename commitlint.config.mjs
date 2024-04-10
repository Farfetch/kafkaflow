export default {
  extends: ['@commitlint/config-conventional'],
  ignores: [(message) => /^Bumps \[(.+)\]\((.+)\)(.*).$/m.test(message)],
  rules: {
    'body-max-line-length': [0, 'always'],
  }  
}