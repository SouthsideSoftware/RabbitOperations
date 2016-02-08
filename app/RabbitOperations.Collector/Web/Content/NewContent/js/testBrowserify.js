var _ = require('underscore'),
  names = ['Bruce Wayne', 'Wally West', 'John Jones', 'Kyle Rayner', 'Arthur Curry', 'Clark Kent'],
  otherNames = ['Barry Allen', 'Hal Jordan', 'Kara Kent', 'Diana Prince', 'Ray Palmer', 'Oliver Queen'];

_.each([names, otherNames], function (nameGroup) {
  findSuperman(nameGroup);
});

function findSuperman(values) {
  _.find(values, function (name) {
    if (name === 'Clark Kent') {
      console.log('It\'s Superman!');
    } else {
      console.log('... No superman!');
    }
  });
}