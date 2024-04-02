export default {
  extends: ['@commitlint/config-conventional'],
  ignores: [(message) => /^Bumps \[.+]\(.+\) from .+ to .+\.$/m.test(message)],
  rules: {
    'body-max-line-length': [0, 'always'],
  }  
}