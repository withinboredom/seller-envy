'use strict';

describe('Navbar', () => {
  var Navbar, component;

  beforeEach(() => {
    Navbar = require('./Navbar.jsx');
    component = new Navbar();
  });

  it('should create a new instance of Navbar', () => {
    expect(component).toBeDefined();
  });
});
