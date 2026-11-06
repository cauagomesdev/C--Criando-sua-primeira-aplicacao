Episodio ep1 = new Episodio(3, "O começo de tudo", 60);
ep1.AdicionarConvidados("João");
ep1.AdicionarConvidados("Maria");
Console.WriteLine(ep1.Resumo);

Episodio ep2 = new Episodio(1, "O meio de tudo", 80);
ep2.AdicionarConvidados("Jorge");
ep2.AdicionarConvidados("Claudia");
ep2.AdicionarConvidados("Lucas");

Episodio ep3 = new Episodio(2, "O fim de tudo", 100);
ep3.AdicionarConvidados("Ana");
ep3.AdicionarConvidados("Pedro");
ep3.AdicionarConvidados("Carla");
ep3.AdicionarConvidados("Rafael");

Podcast podcast = new Podcast("Histórias Incríveis", "Lucas Silva");
podcast.AdicionarEpisodio(ep1);
podcast.AdicionarEpisodio(ep2);
podcast.AdicionarEpisodio(ep3);
podcast.ExibirDetalhes();