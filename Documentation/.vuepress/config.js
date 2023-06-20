module.exports = {
    title: 'Test Automation Operator Panel',
    description: 'An operator panel for',
    
    themeConfig: {
        repo: 'https://github.com/opentap/operator-panel',
        editLinks: true,
        editLinkText: 'Help improve this page!',
        docsBranch: 'main',
        docsDir: 'Documentation',
        nav: [
            { text: 'OpenTAP', link: 'https://github.com/opentap/opentap' },
            { text: 'OpenTAP Homepage', link: 'https://www.opentap.io' }
        ],
        sidebar: [
            ['/', "Readme"],
            
        ]
    },
    dest: '../public',
    base: '/Operator Panel/'
}

